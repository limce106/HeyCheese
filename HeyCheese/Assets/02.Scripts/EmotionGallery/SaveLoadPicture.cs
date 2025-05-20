using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class SaveLoadPicture : MonoBehaviour
{
    /// <summary>
    /// 사진을 캡처하고 저장한 후, 외부에서 전달한 콜백으로 파일 경로와 시간 정보를 넘긴다.
    /// </summary>
    public IEnumerator CaptureAndSave(Action<string, string> onSavedCallback = null)
    {
        yield return new WaitForEndOfFrame();

        // AR 카메라 화면이 있는 영역만 캡처
        float targetAspect = 3f / 4f;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float targetHeight = screenWidth / targetAspect;
        float yOffset = (screenHeight - targetHeight) / 2f;

        float x = 0f;
        float y = yOffset;
        float width = screenWidth;
        float height = targetHeight;

        // y 좌표 보정 (스크린 좌표계는 좌하(0,0) 기준)
        float readY = screenHeight - y - height;

        Texture2D screenImage = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(x, readY, width, height), 0, 0);
        screenImage.Apply();

        string filename = "emotion_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filepath = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllBytes(filepath, screenImage.EncodeToPNG());
        Destroy(screenImage);

        string capturedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        // 외부에 전달
        onSavedCallback?.Invoke(filepath, capturedAt);
    }
}
