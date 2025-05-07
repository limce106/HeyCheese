using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks; // 비동기식 처리를 위함
using System.Linq;

public class FaceRecognition : MonoBehaviour
{
    // 엔드포인트 작성 시 /face/v1.0~ 부분은 제외하고 작성해야 된다는 글이 있었음. https://m.blog.naver.com/ambidext/221407711907
    // POST 방식으로 API 키 전송하며 정보 요청
    // 전송 형태: https://${ENDPOINT}/face/v1.0/detect?returnFace...
    private readonly string SUBSCRIPTION_KEY = "REMOVED"; // API KEY
    private readonly string ENDPOINT = "REMOVED"; // 엔드포인트
    //private readonly string ENDPOINT = "https://eastasia.api.cognitive.microsoft.com/"; // 엔드포인트 - 이건 호출조차 안됨
    private readonly string detectURL = "face/v1.0/detect?";
    // WebCamTexture와 RawImage 컴포넌트
    private WebCamTexture webCamTexture;
    public RawImage rawImage;
    // 감정 텍스트
    public Text emotionText;
    private void Start()
    {
        // 웹캠 초기화
        InitializeWebCam();
    }
    // 웹캠 초기화 및 시작
    private void InitializeWebCam()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            webCamTexture = new WebCamTexture(devices[0].name);
            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;
            webCamTexture.Play(); // 웹캠 시작
        }
        else
        {
            Debug.LogError("No webcam detected!");
        }
    }
    // 버튼이 눌렸을 때 웹캠텍스처를 가져와 바이트로 전환 후 Request할 수 있도록 await
    public async void TakePhoto()
    {
        // 웹캠의 픽셀을 가져와 텍스처에 어플라이
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();
        // 텍스처를 바이트로 전환
        byte[] imageData = photo.EncodeToJPG(); // PNG로 해야 되나?
        await SendToAzure(imageData);
    }
    // Request
    // 코루틴 대신 async를 써서 내부에 await를 사용할 수 있도록 비동기식으로 처리
    public async Task SendToAzure(byte[] imageBytes)
    {
        HttpClient client = new HttpClient();
        // Request Header
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);
        // Request Parameter
        // 감정에 대한 값만 필요해서 emotion에 대해서만 파라미터 작성
        //string requestParameters = "returnFaceAttributes=emotion";
        //string requestParameters = "returnFaceLandmarks=false&returnFaceAttributes=emotion&recognitionModel=recognition_03&faceIdTimeToLive=60"; // 모두 작성 시 이런 형태
        string requestParameters = "returnFaceLandmarks=false&returnFaceAttributes=emotion&recognitionModel=recognition_03";
        // Request URI
        // requestURI = REMOVEDface/v1.0/detect?returnFaceAttributes=emotion
        string requestURI = ENDPOINT + detectURL + requestParameters;
        Debug.Log("requestURI: " + requestURI);
        // Request Body
        // Post stores JPEG image
        // (주의) azure는 JPEG, PNG, GIF 또는 BMP 형식의 이미지만 처리 가능
        using (var content = new ByteArrayContent(imageBytes))
        {
            // content type으로 "application/octet-stream", "application/json", "multipart/form-data"가 있음
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            // REST API 호출 실행
            HttpResponseMessage response = await client.PostAsync(requestURI, content);
            if (response.IsSuccessStatusCode) // status code가 200일 경우
            {
                // Get JSON Response
                string responseBody = await response.Content.ReadAsStringAsync();
                // JSON 파싱
                ParseEmotionData(responseBody);
            }
            else
            {
                Debug.LogError("Face API 호출 실패: " + response.StatusCode + " - " + response.ReasonPhrase);
                string errorResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorResponse);
            }
        }
    }
    // Face Request 이후 반환 데이터인 JSON 파싱
    // 얼굴의 수를 파악하여 하나의 얼굴만 있을 때 작동하도록 함
    private void ParseEmotionData(string json)
    {
        try
        {
            FaceList faceList = JsonUtility.FromJson<FaceList>("{\"faces\":" + json + "}"); // "faces"키로 JSON 응답을 감쌈
            if (faceList.faces.Length == 1) // 얼굴이 하나만 있을 때만 실행
            {
                Emotion emotion = faceList.faces[0].faceAttributes.emotion; // 감정 분석 결과
                // 상위 감정 표시
                // 감정을 딕셔너리화 한 후 sorting
                var emotionDict = new Dictionary<string, float>
                {
                    { "분노", emotion.anger },
                    { "경멸", emotion.contempt },
                    { "혐오", emotion.disgust },
                    { "두려움", emotion.fear },
                    { "기쁨", emotion.happiness },
                    { "중립", emotion.neutral },
                    { "슬픔", emotion.sadness },
                    { "놀람", emotion.surprise }
                };
                var highestEmotion = emotionDict.OrderByDescending(e => e.Value).First();
                Debug.Log($"가장 강한 감정: {highestEmotion.Key} ({highestEmotion.Value:P})");
                // 모든 감정 표시
                //Debug.Log($"기쁨: {emotion.happiness}, 슬픔: {emotion.sadness}, 분노: {emotion.anger}");
                DisplayEmotion(emotion);
            }
            else if (faceList.faces.Length > 1)
            {
                Debug.LogWarning("여러 개의 얼굴이 감지되었습니다. 한 개의 얼굴만 화면에 배치해주세요.");
            }
            else
            {
                Debug.Log("얼굴을 찾지 못했습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON 파싱 오류: " + e.Message);
        }
    }
    // 모든 감정 표시
    // 매개변수: emotion
    private void DisplayEmotion(Emotion emotion)
    {
        emotionText.text =
            $"Anger: {emotion.anger:F2}\n" +
            $"Contempt: {emotion.contempt:F2}\n" +
            $"Disgust: {emotion.disgust:F2}\n" +
            $"Fear: {emotion.fear:F2}\n" +
            $"Happiness: {emotion.happiness:F2}\n" +
            $"Neutral: {emotion.neutral:F2}\n" +
            $"Sadness: {emotion.sadness:F2}\n" +
            $"Surprise: {emotion.surprise:F2}";
    }
    // 얼굴이 1개만 검출되도록
    [Serializable]
    public class FaceList
    {
        public Face[] faces;
    }
    // Face API에서 반환되는 얼굴 데이터 클래스
    [Serializable]
    public class Face
    {
        // public string faceId;
        // public FaceRectangle faceRectangle;
        public FaceAttributes faceAttributes;
    }
    [Serializable]
    public class FaceAttributes
    {
        public Emotion emotion;
    }
    [Serializable]
    public class Emotion
    {
        public float anger;
        public float contempt;
        public float disgust;
        public float fear;
        public float happiness;
        public float neutral;
        public float sadness;
        public float surprise;
    }
}
