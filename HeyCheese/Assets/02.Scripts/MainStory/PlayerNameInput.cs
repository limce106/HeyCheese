using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    public MainStoryManager MainStoryManager;

    public GameObject inputfield;
    public TMP_InputField nameInputField;
    public GameObject confirmationPanel;
    public Button confrimationBtn;
    public TMP_Text confirmationText;
    public GameObject nameWarningText;

    private string pendingName;
    public static string PlayerName { get; private set; }

    // 애니메이션
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 10f;

    void Start()
    {
        nameInputField.characterLimit = 8;

        nameWarningText.SetActive(false);
        confirmationPanel.SetActive(false);
    }

    // 입력 완료 처리(입력 완료 버튼)
    public void OnNameInputComplete()
    {
        pendingName = nameInputField.text.Trim();

        // 입력값이 없으면 무시
        if (string.IsNullOrEmpty(pendingName))
        {
            // 경고 메시지 띄우며 inputfield 흔들기
            nameWarningText.SetActive(true);
            ShakeInputField();
            return;
        }

        // 입력이 정상인 경우
        confirmationPanel.SetActive(true);
        nameWarningText.SetActive(false);
        nameInputField.interactable = false;
        confrimationBtn.interactable = false;

        confirmationText.text = $"\"{pendingName}\"";
        confirmationPanel.SetActive(true);
    }

    // 이름 입력 확인 시
    public void OnConfirmYes()
    {
        PlayerName = pendingName;

        // 이름 저장
        //PlayerPrefs.SetString("PlayerName", PlayerName);
        //PlayerPrefs.Save();
        //Debug.Log($"플레이어 이름 저장됨: {PlayerName}");
        PlayerDataManager.Instance.SetPlayerName(pendingName);

        confirmationPanel.SetActive(false);

        // MainStoryManager의 NextStep() 호출
        PlayerDataManager.Instance.LoadPlayerName(); // 이름 로드
        MainStoryManager.PlayerName = PlayerDataManager.Instance.PlayerName; // 이름 가져오기
        MainStoryManager.NextStep();

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

    // 애니메이션
    //public void ShakeInputField()
    //{
    //    inputfield.transform.DOShakePosition(0.3f, new Vector3(10f, 0, 0));
    //}
    public void ShakeInputField()
    {
        StartCoroutine(Shake(inputfield.transform));
    }

    IEnumerator Shake(Transform target)
    {
        Vector3 originalPos = target.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            target.localPosition = originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localPosition = originalPos;
    }
}
