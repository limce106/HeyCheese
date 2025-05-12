#if UNITY_ANDROID
using UnityEngine.Android;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class EmotionDetector : MonoBehaviour
{
    static readonly Dictionary<string, int> likelihoodToPercent = new Dictionary<string, int>
    {
        { "UNKNOWN", 0 },
        { "VERY_UNLIKELY", 10 },
        { "UNLIKELY", 25 },
        { "POSSIBLE", 50 },
        { "LIKELY", 75 },
        { "VERY_LIKELY", 100 }
    };

    public RawImage webcamDisplay;
    public Text emotionText;
    private WebCamTexture webCamTexture;

    private bool isUsingWebcam = false;
    private bool isFrontFacing = false;
    private bool hasCaptured = false;

    void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        isUsingWebcam = true;

        webcamDisplay.rectTransform.localEulerAngles = Vector3.zero;
        webcamDisplay.rectTransform.localScale = Vector3.one;

        StartWebcam();
#else
        isUsingWebcam = false;
        // ARCore 카메라는 자동 실행됨 (ARCameraBackground, ARSession이 처리)
        // 웹캠 사용 X
#endif
    }

    // 컴퓨터 웹캠 감지
    // 컴퓨터 테스트 시 웹캠 사용
    void StartWebcam()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogWarning("카메라를 찾을 수 없습니다.");
            return;
        }

        string selectedDeviceName = devices[0].name;
        foreach (var device in devices)
        {
            if (device.isFrontFacing)
            {
                selectedDeviceName = device.name;
                break;
            }
        }

        webCamTexture = new WebCamTexture(selectedDeviceName, 600, 800);
        webcamDisplay.texture = webCamTexture;
        webcamDisplay.rectTransform.localEulerAngles = Vector3.zero;
        webCamTexture.Play();
    }

    void Update()
    {
        if (isUsingWebcam) // 웹캠 사용 시
        {
            if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
            {
                // 웹캠에서 새로운 프레임이 들어왔을 때 RawImage에 텍스처 할당
                webcamDisplay.texture = webCamTexture;

                // 회전 보정 제거: RawImage 회전은 더 이상 하지 않음
                // (회전 각도 적용 안함)
                int rotation = webCamTexture.videoRotationAngle;
                webcamDisplay.rectTransform.localEulerAngles = new Vector3(0, 0, -rotation);
            }
        }
    }

    // 카메라 버튼 클릭 시
    public void OnClick_DetectEmotion()
    {
        Debug.Log("감정 분석 시작");
        if (isUsingWebcam)
        {
            StartCoroutine(CaptureAndDetect_Webcam());
        }
        else
        {
            StartCoroutine(CaptureAndDetect_ARCamera());
        }
    }

    // PC/Editor용 웹캠 사용 시 표정 분석
    IEnumerator CaptureAndDetect_Webcam()
    {
        while (!webCamTexture.didUpdateThisFrame)
            yield return null;

        int width = webCamTexture.width;
        int height = webCamTexture.height;

        if (width < 100 || height < 100)
        {
            Debug.LogError("웹캠이 아직 초기화되지 않았습니다.");
            yield break;
        }

        Texture2D photo = new(width, height, TextureFormat.RGB24, false);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        webCamTexture.Stop();

        Texture2D finalPhoto = RotateAndMirrorTexture(photo, webCamTexture.videoRotationAngle, webCamTexture.videoVerticallyMirrored);

        // UI에 사진 표시
        webcamDisplay.texture = finalPhoto;
        webcamDisplay.rectTransform.localEulerAngles = Vector3.zero;
        webcamDisplay.rectTransform.localScale = Vector3.one;

        // 감정 분석 시작
        string base64Image = EncodeImageToBase64(finalPhoto);
        StartCoroutine(CallVisionAPI(base64Image));
    }

    // Android 기기 카메라 사용 시 표정 분석
    IEnumerator CaptureAndDetect_ARCamera()
    {
        yield return new WaitForEndOfFrame(); // AR 카메라 렌더 후 캡처

        // RawImage의 RectTransform을 기준으로 스크린 좌표 계산
        RectTransform rt = webcamDisplay.rectTransform;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners); // [0]: bottom left, [1] top left, [2] top right, [3] bottom right

        // 화면 좌표로 변환 (BottomLeft 기준)
        float x = corners[0].x;
        float y = corners[0].y;
        Debug.Log("corners[0].x = " + x + "corners[0].y = " + y);
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;
        Debug.Log("width = " + width + "height = " + height);
        Debug.Log("screen height = " + Screen.height);

        // y축은 아래가 0, 위에가 height인 스크린 좌표 기준이라 뒤집어줘야 함
        y = Screen.height - y - height;
        Debug.Log("Reversed y = " + y);

        // 캡처
        Texture2D photo = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(x, y, width, height), 0, 0);
        photo.Apply();

        webcamDisplay.texture = photo;
        webcamDisplay.rectTransform.localEulerAngles = Vector3.zero;

        string base64Image = EncodeImageToBase64(photo);
        StartCoroutine(CallVisionAPI(base64Image));
    }


    Texture2D RotateAndMirrorTexture(Texture2D original, int angle, bool mirrorHorizontal)
    {
        int width = original.width;
        int height = original.height;
        Texture2D rotated = (angle == 90 || angle == 270) ? new Texture2D(height, width) : new Texture2D(width, height);

        Color[] originalPixels = original.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = originalPixels[y * width + x];
                int newX = x, newY = y;

                switch (angle)
                {
                    case 90: newX = height - y - 1; newY = x; break;
                    case 180: newX = width - x - 1; newY = height - y - 1; break;
                    case 270: newX = y; newY = width - x - 1; break;
                }

                if (mirrorHorizontal) newX = rotated.width - newX - 1;

                if (newX >= 0 && newX < rotated.width && newY >= 0 && newY < rotated.height)
                    rotated.SetPixel(newX, newY, color);
            }
        }

        rotated.Apply();
        return rotated;
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
        string apiKey = "AIzaSyAtycWvjH2Pr-72WBeCSUdTYHsFHbKLE50";
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
            else // API 요청 성공
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

        int percent = likelihoodToPercent.ContainsKey(bestLikelihood) ? likelihoodToPercent[bestLikelihood] : 0;

        Debug.Log($"Best Emotion: {bestEmotion}");  // 최종 감정 확인
        return $"{bestEmotion} ({percent}%)";
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