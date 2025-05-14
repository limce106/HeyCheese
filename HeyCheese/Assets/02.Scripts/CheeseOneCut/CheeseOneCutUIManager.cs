using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class CheeseOneCutUIManager : MonoBehaviour
{
    public GameObject framePanel;
    public GameObject filterPanel;
    public GameObject hiddenMissionPanel;

    public Button cancelButton;
    public Button cameraButton;

    public EmotionGalleryDBWriter emotionGalleryDBWriter;

    [SerializeField]
    private ARFaceManager arFaceManager;
    public SaveLoadPicture saveLoadPicture;

    public void OnClick_Gallery()
    {
        SceneManager.LoadScene("EmotionGallery");
    }

    public void OnClick_Camera()
    {
        int faceCountAtCapture = arFaceManager.trackables.count;
        StartCoroutine(CaptureAndCheckMission(faceCountAtCapture));
    }

    public void OnClick_Frame()
    {
        UpdateFrameButtons();
        framePanel.SetActive(true);

        cancelButton.interactable = true;
    }

    public void OnClick_Filter()
    {
        UpdateFilterButtons();
        filterPanel.SetActive(true);

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
        var filterUnlockedItems = FilterFrameManager.instance.GetFilterUnlockedReadOnly();
        UpdateButtons(filterButtons, filterUnlockedItems, "5AR");
    }

    private void UpdateFrameButtons()
    {
        Button[] frameButtons = framePanel.GetComponentsInChildren<Button>();
        var frameUnlockedItems = FilterFrameManager.instance.GetFrameUnlockedReadOnly();
        UpdateButtons(frameButtons, frameUnlockedItems, "6Frame");
    }

    private void UpdateButtons(Button[] buttons, ReadOnlyDictionary<string, bool> items, string resourcesPath)
    {
        string[] itemKeys = items.Keys.ToArray();
        bool[] itemValues = items.Values.ToArray();

        for (int i = 0; i < itemValues.Length; i++)
        {
            if (itemValues[i] == false)
            {
                // RemoveFilter 버튼 제외하여 인덱스 + 1
                // 사용 불가능한 버튼은 상호작용 불가능
                buttons[i + 1].interactable = false;
            }

            string itemName = itemKeys[i];

            Sprite itemSprite = Resources.Load<Sprite>($"{resourcesPath}/{itemName}");
            Image buttonImage = buttons[i + 1].GetComponent<Image>();
            buttonImage.sprite = itemSprite;

            buttons[i + 1].gameObject.name = itemName;
        }
    }

    private IEnumerator CaptureAndCheckMission(int faceCount)
    {
        // 캡처가 끝난 후 히든미션 패널이 뜨게 한다.
        // (캡처된 사진에 히든미션 패널이 같이 찍히는 것을 방지하기 위함)
        yield return StartCoroutine(saveLoadPicture.CaptureAndSave((filepath, capturedAt) =>
        {
            emotionGalleryDBWriter.InsertFreePhoto(filepath, capturedAt);
        }));

        FilterFrameManager.instance.CheckHiddenMission(faceCount);
    }
}
