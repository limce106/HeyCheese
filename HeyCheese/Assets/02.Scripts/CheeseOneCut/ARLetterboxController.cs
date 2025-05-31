using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARLetterboxController : MonoBehaviour
{
    public Image topLetterbox;
    public Image bottomLetterbox;
    public Image leftLetterbox;
    public Image rightLetterbox;
    void Start()
    {
        ApplyLetterbox();
    }

    void ApplyLetterbox()
    {
        float targetAspect = 3f / 4f;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth / screenHeight;

        if (screenAspect > targetAspect)
        {
            // 화면이 더 가로로 길면 좌우에 레터박스(pillarbox) 필요
            float targetWidth = screenHeight * targetAspect;
            float sideBoxWidth = (screenWidth - targetWidth) / 2f;

            leftLetterbox.rectTransform.sizeDelta = new Vector2(sideBoxWidth, screenHeight);
            rightLetterbox.rectTransform.sizeDelta = new Vector2(sideBoxWidth, screenHeight);

            leftLetterbox.rectTransform.anchoredPosition = new Vector2(-((screenWidth / 2f) - (sideBoxWidth / 2f)), 0);
            rightLetterbox.rectTransform.anchoredPosition = new Vector2((screenWidth / 2f) - (sideBoxWidth / 2f), 0);

            leftLetterbox.gameObject.SetActive(true);
            rightLetterbox.gameObject.SetActive(true);
            topLetterbox.gameObject.SetActive(false);
            bottomLetterbox.gameObject.SetActive(false);
        }
        else if (screenAspect < targetAspect)
        {
            // 화면이 더 세로로 길면 위아래에 레터 박스 필요
            float targetHeight = screenWidth / targetAspect;
            float boxHeight = (screenHeight - targetHeight) / 2f;

            topLetterbox.rectTransform.sizeDelta = new Vector2(screenWidth, boxHeight);
            bottomLetterbox.rectTransform.sizeDelta = new Vector2(screenWidth, boxHeight);

            topLetterbox.rectTransform.anchoredPosition = new Vector2(0, (screenHeight / 2f) - (boxHeight / 2f));
            bottomLetterbox.rectTransform.anchoredPosition = new Vector2(0, -((screenHeight / 2f) - (boxHeight / 2f)));

            topLetterbox.gameObject.SetActive(true);
            bottomLetterbox.gameObject.SetActive(true);
            leftLetterbox.gameObject.SetActive(false);
            rightLetterbox.gameObject.SetActive(false);
        }
        else
        {
            // 이미 화면 비율이 4:3이면 아무것도 필요 없음
            topLetterbox.gameObject.SetActive(false);
            bottomLetterbox.gameObject.SetActive(false);
            leftLetterbox.gameObject.SetActive(false);
            rightLetterbox.gameObject.SetActive(false);
        }
    }
}
