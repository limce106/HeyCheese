using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class EmotionDetector : MonoBehaviour
{
    public RawImage webcamDisplay;
    public Text emotionText;
    private WebCamTexture webCamTexture;

    private bool hasCaptured = false;

    void Start()
    {
        webCamTexture = new WebCamTexture(640, 480, 30);
        webcamDisplay.texture = webCamTexture;
        webCamTexture.Play();
    }

    void Update()
    {
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            webcamDisplay.texture = webCamTexture;
        }
    }

    public void OnClick_DetectEmotion()
    {
        if (hasCaptured)
        {
            Debug.Log("이미 감정 분석이 완료됨. 다시 실행되지 않음.");
            return;
        }

        Debug.Log("버튼 눌림: 감정 분석 시작");
        hasCaptured = true;

        // 화면 캡처
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        // 웹캠 멈춤
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }

        webcamDisplay.texture = photo;

        string base64Image = EncodeImageToBase64(photo);
        StartCoroutine(CallVisionAPI(base64Image));
    }

    IEnumerator CaptureAndDetect()
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        string base64Image = EncodeImageToBase64(photo);
        StartCoroutine(CallVisionAPI(base64Image));
    }

    string EncodeImageToBase64(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToJPG();
        return System.Convert.ToBase64String(imageBytes);
    }

    IEnumerator CallVisionAPI(string base64Image)
    {
        VisionRequest visionRequest = new VisionRequest
        {
            requests = new List<Request>
            {
                new Request
                {
                    image = new Image { content = base64Image },
                    features = new List<Feature> { new Feature() }
                }
            }
        };

        string jsonData = JsonUtility.ToJson(visionRequest);
        string apiKey = "REMOVED"; // ⚠️ 여기에 실제 API 키 입력
        string url = $"https://vision.googleapis.com/v1/images:annotate?key={apiKey}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                emotionText.text = "API 요청 실패";
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                FaceResponse faceResponse = JsonUtility.FromJson<FaceResponse>(jsonResponse);

                if (faceResponse.responses.Length > 0 && faceResponse.responses[0].faceAnnotations.Length > 0)
                {
                    FaceAnnotation face = faceResponse.responses[0].faceAnnotations[0];
                    string dominantEmotion = GetDominantEmotion(face);
                    emotionText.text = $"감지된 표정: {dominantEmotion}";
                }
                else
                {
                    emotionText.text = "얼굴이 감지되지 않았습니다.";
                }
            }
        }
    }

    string GetDominantEmotion(FaceAnnotation face)
    {
        Dictionary<string, string> emotions = new Dictionary<string, string>
    {
        { "기쁨", face.joyLikelihood },
        { "슬픔", face.sorrowLikelihood },
        { "분노", face.angerLikelihood },
        { "놀람", face.surpriseLikelihood }
    };

        string bestEmotion = "표정 없음";
        string bestLikelihood = "UNKNOWN";

        foreach (var emotion in emotions)
        {
            Debug.Log($"{emotion.Key}: {emotion.Value}");  // 감정과 확률 값 출력
            if (IsMoreLikely(emotion.Value, bestLikelihood))
            {
                bestLikelihood = emotion.Value;
                bestEmotion = emotion.Key;
            }
        }

        Debug.Log($"Best Emotion: {bestEmotion}");  // 최종 감정 확인
        return bestEmotion;
    }

    bool IsMoreLikely(string current, string best)
    {
        string[] order = { "UNKNOWN", "VERY_UNLIKELY", "UNLIKELY", "POSSIBLE", "LIKELY", "VERY_LIKELY" };
        return System.Array.IndexOf(order, current) > System.Array.IndexOf(order, best);
    }
}

#region Vision API Response Classes

[System.Serializable]
public class VisionRequest
{
    public List<Request> requests;
}

[System.Serializable]
public class Request
{
    public Image image;
    public List<Feature> features;
}

[System.Serializable]
public class Image
{
    public string content;
}

[System.Serializable]
public class Feature
{
    public string type = "FACE_DETECTION";
    public int maxResults = 1;
}

[System.Serializable]
public class FaceResponse
{
    public Response[] responses;
}

[System.Serializable]
public class Response
{
    public FaceAnnotation[] faceAnnotations;
}

[System.Serializable]
public class FaceAnnotation
{
    public string joyLikelihood;
    public string sorrowLikelihood;
    public string angerLikelihood;
    public string surpriseLikelihood;
}

#endregion