using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class EduMenuLoader : MonoBehaviour
{
    [Header("UI Elements")]
    // 패널
    public GameObject eduMenuPanel;
    public GameObject situationMenuPanel;
    public GameObject eduPopupMenuPanel;

    // 버튼
    public Button[] emotionButtons; // Emotion 버튼 4개
    public GameObject[] situationButtons; // Situation 버튼 4개 - 이미지 받아서 go

    // ID 값
    private string[] emotionIDs = { "happiness", "sadness", "surprise", "anger" };
    private string selectedEmotionID;

    // Data Structure
    private Dictionary<string, List<EduMenu>> eduMenus = new Dictionary<string, List<EduMenu>>();

    private void Awake()
    {
        // 파싱한 내용 불러오기
        CSVParser csvParser = new CSVParser();
        eduMenus = csvParser.ParseEduMenus();
    }

    private void Start()
    {
        // Emotion 버튼 이벤트 연결
        for(int i = 0; i < emotionButtons.Length; i++)
        {
            string id = emotionIDs[i]; // 클로저 문제 방지 위한 값 복사
            emotionButtons[i].onClick.AddListener(() => OnEmotionSelected(id));
        }

        // 초기 패널 설정
        eduMenuPanel.SetActive(true);
        situationMenuPanel.SetActive(false);
        eduPopupMenuPanel.SetActive(false);
    }

    // EduMenu > SituationMenu
    // EduMenu 중 감정 버튼 선택 시 이벤트 처리
    // 인스펙터 창에서 각 버튼에 연결 및 감정 명시 필요
    // 눌렸을 때 SituationMenu 패널 오픈되며 상황 데이터 눌린 감정에 맞게 전환
    public void OnEmotionSelected(string emotionID)
    {
        // 눌린 감정 확인
        selectedEmotionID = emotionID;

        if (!eduMenus.ContainsKey(emotionID))
        {
            Debug.LogError("해당 감정에 대한 상황이 없습니다: " + emotionID);
            return;
        }

        // 상황에 맞게 버튼 아이콘, 리스너 변경
        var situations = eduMenus[emotionID];
        for (int i = 0; i < situationButtons.Length; i++)
        {
            if (i >= situations.Count) continue;

            EduMenu data = situations[i]; // {emotionID, situationID, iconPath}

            Image iconImage = situationButtons[i].GetComponent<Image>();
            Sprite icon = Resources.Load<Sprite>(data.IconPath);
            //if (icon != null) iconImage.sprite = icon;

            Button btn = situationButtons[i].GetComponent<Button>();
            string situationID = data.SituationID;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSituationSelected(situationID));
        }

        // SituationMenu로 전환
        eduMenuPanel.SetActive(false);
        situationMenuPanel.SetActive(true);
        eduPopupMenuPanel.SetActive(false);
    }

    // SituationMenu > EduPopupMenu
    // SituationMenu 중 상황 버튼 선택 시 이벤트 처리
    // 눌렸을 때 EduPopupMenu 패널 오픈되며 레벨 데이터 눌린 상황에 맞게 전환
    // -- EduStory씬으로 전환 시, 상황, 레벨, 감정 전달 --
    void OnSituationSelected(string situationID)
    {
        Debug.Log("선택된 상황: " + selectedEmotionID + "/" + situationID);
        eduPopupMenuPanel.SetActive(true);
        // 팝업 UI 오픈 및 상황 ID 전달
        //SituationPopupController.Instance.OpenPopup(selectedEmotionID, situationID);
    }
}
