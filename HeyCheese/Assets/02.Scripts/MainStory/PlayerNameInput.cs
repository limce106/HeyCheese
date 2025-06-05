using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    public MainStoryManager MainStoryManager;

    public TMP_InputField nameInputField;
    public GameObject confirmationPanel;
    public Button confrimationBtn;
    public TMP_Text confirmationText;

    public GameObject inputField;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 10f;

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
        pendingName = nameInputField.text.Trim();

        // 입력값이 없다면 경고 메시지를 띄우며 입력 필드를 흔듦
        if (string.IsNullOrEmpty(pendingName))
        {
            ShakeInputField();
            return;
        }

        // 입력이 정상인 경우
        confirmationPanel.SetActive(true);
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
        MainStoryGameManager.MainStoryGM.playerName = PlayerDataManager.Instance.PlayerName; // 이름 가져오기
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

    // 인풋 필드 null일 경우 쉐이크

    public void ShakeInputField()
    {
        StartCoroutine(Shake(nameInputField.transform));
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
