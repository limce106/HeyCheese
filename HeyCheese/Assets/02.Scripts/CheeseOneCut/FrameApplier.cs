using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameApplier : MonoBehaviour
{
    [SerializeField]
    private GameObject frameObj;
    private Image frameImg;
    public GameObject framePanel;
    public GameObject bottomButtons;

    private string frameName;

    void Awake()
    {
        frameImg = frameObj.GetComponent<Image>();
    }

    public void SetFrameName(string name)
    {
        frameName = name;
    }

    public void OnClick_Filter()
    {
        SetFrameName(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        ApplyFrame();
    }

    void ApplyFrame()
    {
        if (frameName == null)
            return;

        if(!frameObj.activeSelf)
        {
            frameObj.SetActive(true);
        }

        string framePath = $"6Frame/{frameName}";
        Sprite frame = Resources.Load<Sprite>(framePath);
        frameImg.sprite = frame;

        if (frameObj.activeSelf == false)
        {
            frameObj.SetActive(true);
        }

        framePanel.SetActive(false);
        bottomButtons.SetActive(true);
    }

    public void RemoveFrame()
    {
        frameObj.SetActive(false);
        framePanel.SetActive(false);
        bottomButtons.SetActive(true);
    }
}
