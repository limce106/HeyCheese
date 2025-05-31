using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class EpisodeMenuLoader : MonoBehaviour
{
    [Header("UI Elements")]
    // 패널
    public Transform episodeBtnContainer; // 하위에 메뉴 버튼 배치
    // 버튼
    public GameObject episodeBtnPrefab; // 메뉴 버튼 프리팹

    // ID 값
    private string selectedEpisodeID;

    // Data Structure
    private Dictionary<string, StoryMenu> storyMenus = new Dictionary<string, StoryMenu>();

    private void Awake()
    {
        // 파싱한 내용 불러오기
        CSVParser csvParser = new CSVParser();
        storyMenus = csvParser.ParseStoryMenus();
    }

    private void Start()
    {
        // 딕셔너리를 에피소드ID로 정렬하여 리스트화
        // Prolog는 맨 앞에 오도록, Episode_뒤에 오는 숫자 기반으로 나열
        // 숫자가 없거나 파싱 실패 시 맨 뒤로 보내짐
        var sortedMenus = storyMenus.Values
            .OrderBy(menu =>
            {
                if (menu.EpisodeID == "Prolog") return -1; // Prolog은 맨 앞

                // Episode/1 형태로 파싱
                string numberPart = new string(menu.EpisodeID.Where(char.IsDigit).ToArray());
                return int.TryParse(numberPart, out int num) ? num : int.MaxValue; // 에피소드 뒤의 숫자를 기준으로 정렬, 숫자 없거나 파싱 실패 시 맨 뒤로 보내기
            })
            .ToList();

        bool previousCleared = true; // prolog는 항상 해금

        // 돌아가며 버튼 생성
        foreach (var menu in sortedMenus)
        {
            string episodeID = menu.EpisodeID;
            string chapterTitle = menu.ChapterTitle; // 값이 StoryMenu 객체라 그 객체를 가져옴
            string imgPath = menu.ImagePath;

            // 버튼 생성
            GameObject newBtnObj = Instantiate(episodeBtnPrefab, episodeBtnContainer);

            // 버튼의 내용물 가져오기
            Button newBtn = newBtnObj.GetComponent<Button>();
            TMP_Text[] texts = newBtnObj.GetComponentsInChildren<TMP_Text>();
            Image episodeImage = newBtnObj.transform.Find("EpisodeImage")?.GetComponent<Image>();
            Image lockImage = newBtnObj.transform.Find("LockImage")?.GetComponent<Image>();

            // 버튼 내용물 채우기(텍스트 설정)
            if (texts.Length >= 2)
            {
                texts[0].text = episodeID;
                texts[1].text = chapterTitle;
            }

            // 버튼 이미지 설정
            if (!string.IsNullOrEmpty(imgPath))
            {
                if (imgPath == "NONE")
                {
                    episodeImage.sprite = null; // 사진 제거
                    episodeImage.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    Sprite newSprite = Resources.Load<Sprite>($"Arts/8Icon/{imgPath}");

                    if (newSprite != null)
                    {
                        episodeImage.color = new Color(1, 1, 1, 1);
                        episodeImage.sprite = newSprite;
                    }
                    else
                        Debug.LogWarning(episodeID + " 메뉴 이미지 로드 실패: " + imgPath);
                }                
            }

            // 해금 여부 확인
            // 지금 에피소드 해금되어있는지, 이전 에피소드는 해금되어 있는지
            // 이전 에피 해금되었으면 지금 에피 해금되어야 함 but 전 에피가 prolog면 지금 에피도 해금되어있어야 진짜 해금 표시 가능

            // 해금 조건: 이전 에피소드 클리어 된 경우에만
            bool isCleared = PlayerPrefs.GetInt($"{episodeID}_Cleared", 0) == 1; 
            bool isUnlocked = previousCleared;

            if (isCleared)  previousCleared = true;
            else            previousCleared = false;

            // 잠긴 에피소드 시 비주얼 처리
            if (lockImage != null) lockImage.gameObject.SetActive(!isUnlocked);
            newBtn.interactable = isUnlocked;

            // 에피소드 메뉴 버튼 선택 시 발생할 이벤트 등록
            string capturedId = episodeID;
            newBtn.onClick.AddListener(() => OnEpisodeSelected(capturedId));

            // PlayerPref 확인
            Debug.Log($"[DEBUG] {episodeID}: {isUnlocked} | PlayerPref = {PlayerPrefs.GetInt($"{episodeID}_Cleared", -1)}");
        }
    }
 
    // StoryMenu > MainStory
    public void OnEpisodeSelected(string episodeID)
    {
        // 눌린 에피소드 확인
        selectedEpisodeID = episodeID;

        if (!storyMenus.ContainsKey(episodeID))
        {
            Debug.LogError("해당 에피소드가 존재하지 않습니다: " + episodeID);
            return;
        }

        // episodeID를 PlayerPrefs에 저장
        //PlayerPrefs.SetString("SelectedEpisodeID", episodeID);
        //PlayerPrefs.Save();
        PlayerPrefsControll.SavePref_SetString("SelectedEpisodeID", episodeID);

        print(PlayerPrefs.GetString("SelectedEpisodeID"));

        // MainStory로 전환
        SceneManager.LoadScene("MainStory");
    }
}
