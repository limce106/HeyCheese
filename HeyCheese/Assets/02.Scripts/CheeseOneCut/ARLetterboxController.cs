using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARLetterboxController : MonoBehaviour
{
    public Image topLetterbox;
    public Image bottomLetterbox;
    void Start()
    {
        float targetAspect = 3f / 4f;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float targetHeight = screenWidth / targetAspect;
        float totalLetterboxHeight = screenHeight - targetHeight;
        float letterboxHeight = totalLetterboxHeight / 2f;

        Debug.Log("Letterbox Height (px): " + letterboxHeight);

        topLetterbox.rectTransform.sizeDelta = new Vector2(screenWidth, letterboxHeight);
        bottomLetterbox.rectTransform.sizeDelta = new Vector2(screenWidth, letterboxHeight);

        topLetterbox.rectTransform.anchoredPosition = new Vector2(0, screenHeight / 2f - letterboxHeight / 2f);
        bottomLetterbox.rectTransform.anchoredPosition = new Vector2(0, -(screenHeight / 2f - letterboxHeight / 2f));
    }

}
