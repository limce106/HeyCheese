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
    [Header("치즈한컷")]
    [SerializeField]
    private GameObject framePanel;
    [SerializeField]
    private GameObject bottomButtons;


    void Awake()
    {
        frameImg = frameObj.GetComponent<Image>();
    }

    public void ApplyFrame(string frameName)
    {
        string framePath = $"6Frame/{frameName}";
        Sprite frame = Resources.Load<Sprite>(framePath);
        if(frame != null )
        {
            frameImg.sprite = frame;
        }
        else
        {
            Debug.Log($"Cannot Find {frameName}!");
            return;
        }

        if (!frameObj.activeSelf)
        {
            frameObj.SetActive(true);
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
}
