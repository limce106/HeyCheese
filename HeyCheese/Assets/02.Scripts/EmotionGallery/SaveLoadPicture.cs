using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class SaveLoadPicture : MonoBehaviour
{
    [Header("UI")]
    public RectTransform panelToCapture;

    /// <summary>
    /// 사진을 캡처하고 저장한 후, 외부에서 전달한 콜백으로 파일 경로와 시간 정보를 넘긴다.
    /// </summary>
    public IEnumerator CaptureAndSave(Action<string, string> onSavedCallback = null)
    {
        yield return new WaitForEndOfFrame();

        // 캡처할 UI 패널의 화면 좌표 계산
        Vector3[] corners = new Vector3[4];
        panelToCapture.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        // 화면 좌표를 픽셀 좌표로 변환
        x = Mathf.Clamp(x, 0, Screen.width);
        y = Mathf.Clamp(y, 0, Screen.height);
        width = Mathf.Clamp(width, 0, Screen.width - x);
        height = Mathf.Clamp(height, 0, Screen.height - y);

        // y 좌표는 아래에서 위로 계산되므로 보정 필요
        y = Screen.height - y - height;

        Texture2D screenImage = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(x, y, width, height), 0, 0);
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
