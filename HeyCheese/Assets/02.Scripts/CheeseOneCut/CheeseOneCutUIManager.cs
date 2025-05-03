using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CheeseOneCutUIManager : MonoBehaviour
{
    public GameObject framePanel;
    public GameObject filterPanel;
    public GameObject hiddenMissionPanel;

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
        cancelButton.interactable = true;
    }

    public void OnClick_Filter()
    {
        filterPanel.SetActive(true);
        UpdateFilterButtons();

        cancelButton.interactable = true;
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

        cancelButton.interactable = false;
    }

    // 필터 버튼 이름, 이미지 설정
    private void UpdateFilterButtons()
    {
        Button[] filterButtons = filterPanel.GetComponentsInChildren<Button>();
        string[] filterUnlockedKeys = FilterManager.instance.GetFilterUnlockedKeys();
        bool[] filterUnlockedValues = FilterManager.instance.GetFilterUnlockedValues();

        for (int i = 0; i < filterUnlockedValues.Length; i++)
        {
            if (filterUnlockedValues[i] == false)
            {
                // RemoveFilter 버튼 제외하여 인덱스 + 1
                // 사용 불가능한 버튼은 상호작용 불가능
                filterButtons[i + 1].interactable = false;
            }

            string filterName = filterUnlockedKeys[i];

            Sprite filterSprite = Resources.Load<Sprite>($"Arts/5AR/{filterName}/{filterName}");
            UnityEngine.UI.Image buttonImage = filterButtons[i + 1].GetComponent<UnityEngine.UI.Image>();
            buttonImage.sprite = filterSprite;

            filterButtons[i + 1].gameObject.name = filterName;
        }
    }
}
