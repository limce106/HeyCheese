using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public GameObject confirmationPanel;
    public Button confrimationBtn;
    public TMP_Text confirmationText;

    private string pendingName;
    public static string PlayerName { get; private set; }

    void Start()
    {
        nameInputField.characterLimit = 8;

        confirmationPanel.SetActive(false);
    }

    // 입력 완료 처리(입력 완료 버튼)
    public void OnNameInputComplete()
    {
        confirmationPanel.SetActive(true);
        nameInputField.interactable = false;
        confrimationBtn.interactable = false;

        pendingName = nameInputField.text.Trim();

        if (!string.IsNullOrEmpty(pendingName))
        {
            confirmationText.text = $"\"{pendingName}\"";
            confirmationPanel.SetActive(true);
        }
    }

    // 이름 입력 확인 시
    public void OnConfirmYes()
    {
        PlayerName = pendingName;

        // 이름 저장
        PlayerPrefs.SetString("PlayerName", PlayerName);
        PlayerPrefs.Save();
        Debug.Log($"플레이어 이름 저장됨: {PlayerName}");

        //confirmationPanel.SetActive(false);

    }
    // 이름 입력 취소 시
    public void OnConfirmNo()
    {
        confirmationPanel.SetActive(false);
        nameInputField.interactable = true;
        confrimationBtn.interactable = true;

        nameInputField.text = pendingName;
        nameInputField.ActivateInputField(); // 다시 입력받게 포커스 줌
    }
}
