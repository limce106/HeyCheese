using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameApplier : MonoBehaviour
{
    [Header("스토리 & 치즈한컷")]
    [SerializeField]
    // 프레임을 띄울 이미지 오브젝트
    private GameObject frameObj;
    // 이미지 오브젝트 내 이미지 컴포넌트
    private Image frameImg;
    private RectTransform canvasRect;
    [Header("치즈한컷")]
    [SerializeField]
    private GameObject framePanel;
    [SerializeField]
    private GameObject bottomButtons;

    public Image topLetterbox;
    public Image bottomLetterbox;
    public Image leftLetterbox;
    public Image rightLetterbox;

    void Awake()
    {
        frameImg = frameObj.GetComponent<Image>();
        canvasRect = frameImg.canvas.GetComponent<RectTransform>();
    }

    public void ApplyFrame(string frameName)
    {
        string framePath = $"6Frame/{frameName}";
        Sprite frame = Resources.Load<Sprite>(framePath);
        if(frame != null)
        {
            frameImg.sprite = frame;

            GetFrameSizeAndPosition(out Vector2 size, out Vector2 pos);
            frameImg.rectTransform.sizeDelta = size;
            frameImg.rectTransform.anchoredPosition = pos;

            if (!frameObj.activeSelf)
                frameObj.SetActive(true);
        }
        else
        {
            Debug.Log($"Cannot Find {frameName}!");
            return;
        }
    }

    public void RemoveFrame()
    {
        frameObj.SetActive(false);
    }

    public void OnClick_Frame()
    {
        string clickedFrameName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        ApplyFrame(clickedFrameName);
        ActiveBottomButtons();
    }

    public void ActiveBottomButtons()
    {
        framePanel.SetActive(false);
        bottomButtons.SetActive(true);
    }

    private void GetFrameSizeAndPosition(out Vector2 size, out Vector2 position)
    {
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        float targetAspect = 3f / 4f;
        float currentAspect = canvasWidth / canvasHeight;

        if (currentAspect > targetAspect)
        {
            // 가로가 더 넓으면 세로 기준
            float targetWidth = Screen.width - leftLetterbox.rectTransform.sizeDelta.x - rightLetterbox.rectTransform.sizeDelta.x;
            float xOffset = (canvasWidth - targetWidth) / 2f;

            size = new Vector2(targetWidth, canvasHeight);
            position = new Vector2(xOffset - (canvasWidth / 2f - targetWidth / 2f), 0);
        }
        else
        {
            // 세로가 더 길거나 같으면 가로 기준
            float targetHeight = Screen.height - topLetterbox.rectTransform.sizeDelta.y - bottomLetterbox.rectTransform.sizeDelta.y;
            float yOffset = (canvasHeight - targetHeight) / 2f;

            size = new Vector2(canvasWidth, targetHeight);
            position = new Vector2(0, yOffset - (canvasHeight / 2f - targetHeight / 2f));
        }
    }
}
