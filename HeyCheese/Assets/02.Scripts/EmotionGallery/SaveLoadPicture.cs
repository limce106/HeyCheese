using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class SaveLoadPicture : MonoBehaviour
{
    public Image topLetterbox;
    public Image bottomLetterbox;
    public Image leftLetterbox;
    public Image rightLetterbox;

    /// <summary>
    /// 사진을 캡처하고 저장한 후, 외부에서 전달한 콜백으로 파일 경로와 시간 정보를 넘긴다.
    /// </summary>
    public IEnumerator CaptureAndSave(Action<string, string> onSavedCallback = null)
    {
        yield return new WaitForEndOfFrame();

        // AR 카메라 화면이 있는 영역만 캡처

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth / screenHeight;
        float targetAspect = 3f / 4f; // 4:3 (세로가 긴 비율)

        float x = 0f, y = 0f, width = screenWidth, height = screenHeight;

        if (screenAspect > targetAspect)
        {
            // 가로가 더 긴 경우 좌우 잘라냄
            Vector3[] leftCorners = new Vector3[4];
            Vector3[] rightCorners = new Vector3[4];
            leftLetterbox.rectTransform.GetWorldCorners(leftCorners);
            rightLetterbox.rectTransform.GetWorldCorners(rightCorners);

            float leftX = leftCorners[2].x;   // 오른쪽 모서리
            float rightX = rightCorners[0].x; // 왼쪽 모서리

            x = leftX;
            width = rightX - leftX;
            y = 0;
            height = screenHeight;
        }
        else if (screenAspect < targetAspect)
        {
            // 세로가 더 긴 경우 상하 잘라냄
            Vector3[] topCorners = new Vector3[4];
            Vector3[] bottomCorners = new Vector3[4];
            topLetterbox.rectTransform.GetWorldCorners(topCorners);
            bottomLetterbox.rectTransform.GetWorldCorners(bottomCorners);

            float bottomY = bottomCorners[2].y; // 위쪽 모서리
            float topY = topCorners[0].y;       // 아래쪽 모서리

            y = bottomY;
            height = topY - bottomY;
            x = 0;
            width = screenWidth;
        }
        else
        {
            // 4:3 화면과 동일하면 전체 캡처
            x = 0;
            y = 0;
            width = screenWidth;
            height = screenHeight;
        }

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
