using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheeseOneCutButtons : MonoBehaviour
{
    public GameObject framePanel;
    public GameObject filterPanel;

    public Button cancelButton;
    public Button cameraButton;

    public SaveLoadPicture saveLoadPicture;

    private void Start()
    {
        cameraButton.onClick.AddListener(() => StartCoroutine(saveLoadPicture.CaptureAndSave()));
    }

    public void OnClick_Gallery()
    {
        // 갤러리로 이동
    }

    public void OnClick_Frame()
    {
        framePanel.SetActive(true);
        cancelButton.enabled = true;
    }

    public void OnClick_Filter()
    {
        filterPanel.SetActive(true);
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
}
