using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FaceRecognition : MonoBehaviour
{
    // WebCamTexture와 RawImage 컴포넌트
    private WebCamTexture webCamTexture;
    public RawImage rawImage;

    // Face API 엔드포인트 및 API 키
    private string apiUrl = "https://emotiongame-faceapi.cognitiveservices.azure.com/face/v1.0";  // 엔드포인트 URL
    private string subscriptionKey = "7fSTMDc8ZNnD2LQACotnHjTNLfDevUpVYHqhY7Wj7UROKkzw2GCwJQQJ99BCAC3pKaRXJ3w3AAAKACOGyOUU";  // 구독 키
    public Text resultText; // 표정 결과를 출력할 UI 텍스트

    private void Start()
    {
        // 웹캠 초기화
        InitializeWebCam();
    }

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

    private void Update()
    {
        // 웹캠이 정상적으로 실행되고 있는지 확인
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            if (webCamTexture.didUpdateThisFrame)
            {
                // 실시간으로 얼굴 인식
                DetectFaceExpression(webCamTexture);
            }
        }
    }

    public async void DetectFaceExpression(WebCamTexture webCamTexture)
    {
        // WebCamTexture에서 이미지를 추출
        Texture2D frame = new Texture2D(webCamTexture.width, webCamTexture.height);
        frame.SetPixels(webCamTexture.GetPixels());
        frame.Apply();

        // 이미지를 ByteArray로 변환
        byte[] imageBytes = frame.EncodeToJPG();

        try
        {
            // Face API로 요청
            var faceExpressions = await GetFaceExpressions(imageBytes);

            // 표정 분석 결과를 텍스트로 표시
            if (faceExpressions != null && faceExpressions.Length > 0)
            {
                string expression = faceExpressions[0].faceAttributes.emotion.ToString();
                resultText.text = "Detected Expression: " + expression;
                Debug.Log("Detected Expression: " + expression);
            }
            else
            {
                resultText.text = "No face detected!";
                Debug.LogError("No face detected.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Face API 호출 실패: " + ex.Message);
        }
    }

    // 얼굴 감지 및 표정 분석
    private async Task<Face[]> GetFaceExpressions(byte[] imageData)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

        using (var content = new ByteArrayContent(imageData))
        {
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            var response = await client.PostAsync(apiUrl + "/detect?returnFaceAttributes=emotion", content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonUtility.FromJson<FaceList>("{\"faces\":" + responseBody + "}").faces;
            }
            else
            {
                Debug.LogError("Face API 호출 실패: " + response.ReasonPhrase);
                return null;
            }
        }
    }

    // Face API에서 반환되는 얼굴 데이터 클래스
    [Serializable]
    public class Face
    {
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

    [Serializable]
    public class FaceList
    {
        public Face[] faces;
    }
}
