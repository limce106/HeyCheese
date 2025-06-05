using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryImageChanger : MonoBehaviour
{
    void SetImage(UnityEngine.UI.Image targetImg, string imgPath)
    {
        if (string.IsNullOrEmpty(imgPath)) // 사진 존재 x 시
        {
            return; // 사진 변경x
        }

        Sprite newSprite = Resources.Load<Sprite>($"Arts/{imgPath}");
        if (newSprite != null)
        {
            targetImg.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"[ImageChanger] 이미지 로드 실패: {imgPath}, 기본 이미지 사용");

            if (targetImg.name.Contains("background"))
            {
                SetDefaultBackground(targetImg);
            }
        }
    }

    void SetDefaultBackground(UnityEngine.UI.Image targetImg)
    {
        Sprite defaultSprite = Resources.Load<Sprite>($"Arts/2Back//defaultBackground");
        if (defaultSprite != null)
        {
            targetImg.sprite = defaultSprite;
        }
    }
}
