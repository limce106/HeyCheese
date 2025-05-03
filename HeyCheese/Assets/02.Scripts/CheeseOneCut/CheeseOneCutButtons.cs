using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CheeseOneCutButtons : MonoBehaviour
{
    public GameObject framePanel;
    public GameObject filterPanel;

    public Button cancelButton;
    public Button cameraButton;

    [SerializeField]
    private ARFaceManager arFaceManager;
    public SaveLoadPicture saveLoadPicture;

    public void OnClick_Gallery()
    {
        // 갤러리로 이동
    }

    public void OnClick_Camera()
    {
        FilterManager.instance.CheckHiddenMission(arFaceManager.trackables.count);
        StartCoroutine(saveLoadPicture.CaptureAndSave());
    }

    public void OnClick_Frame()
    {
        framePanel.SetActive(true);
        cancelButton.enabled = true;
    }

    public void OnClick_Filter()
    {
        filterPanel.SetActive(true);
        SetFilterButtons();

        cancelButton.enabled = true;
    }

    public void OnClick_Cancel()
    {
        if(framePanel.activeSelf)
        {
            framePanel.SetActive(false);
        }
        else if(filterPanel.activeSelf)
        {
            filterPanel.SetActive(false);
        }

        cancelButton.enabled = false;
    }

    private void SetFilterButtons()
    {
        Button[] filterButtons = filterPanel.GetComponentsInChildren<Button>();
        string[] filterUnlockedKeys = FilterManager.instance.GetFilterUnlockedKeys();
        bool[] filterUnlockedValues = FilterManager.instance.GetFilterUnlockedValues();

        for (int i = 0; i < filterUnlockedValues.Length; i++)
        {
            if (filterUnlockedValues[i] == false)
            {
                // RemoveFilter 버튼 제외하여 인덱스 + 1
                filterButtons[i + 1].gameObject.SetActive(false);
            }
            else
            {
                string filterName = filterUnlockedKeys[i];

                Sprite filterSprite = Resources.Load<Sprite>($"Arts/5AR/{filterName}/{filterName}");
                UnityEngine.UI.Image buttonImage = filterButtons[i + 1].GetComponent<UnityEngine.UI.Image>();
                buttonImage.sprite = filterSprite;

                filterButtons[i + 1].gameObject.name = filterName;
            }
        }
    }
}
