using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
        print(storyMenus.Keys);

        foreach (var entry in storyMenus)
        {
            //print("버튼 생성");
            string episodeID = entry.Key;
            StoryMenu info = entry.Value; // 값이 StoryMenu 객체라 그 객체를 가져옴

            // 버튼 생성
            GameObject newBtnObj = Instantiate(episodeBtnPrefab, episodeBtnContainer);

            // 버튼의 내용물 가져오기
            Button newBtn = newBtnObj.GetComponent<Button>();
            TMP_Text[] texts = newBtnObj.GetComponentsInChildren<TMP_Text>();
            Image episodeImage = newBtnObj.transform.Find("EpisodeImage")?.GetComponent<Image>();

            // 버튼 내용물 채우기
            if (texts.Length >= 2)
            {
                texts[0].text = episodeID;
                texts[1].text = info.ChapterTitle;
            }

            // 버튼 이미지 설정
            if (!string.IsNullOrEmpty(info.ImagePath))
            {
                if (info.ImagePath == "NONE")
                {
                    episodeImage.sprite = null; // 사진 제거
                    episodeImage.color = new Color(1, 1, 1, 0);
                    continue;
                }

                Sprite newSprite = Resources.Load<Sprite>($"Arts/8Icon/{info.ImagePath}");

                if (newSprite != null)
                {
                    episodeImage.color = new Color(1, 1, 1, 1);
                    episodeImage.sprite = newSprite;
                }
                else
                    Debug.LogWarning(info.EpisodeID + " 메뉴 이미지 로드 실패: " + info.ImagePath);
            }

            newBtn.onClick.AddListener(() => OnEpisodeSelected(episodeID));
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
        PlayerPrefs.SetString("SelectedEpisodeID", episodeID);
        PlayerPrefs.Save();

        print(PlayerPrefs.GetString("SelectedEpisodeID"));

        // MainStory로 전환
        SceneManager.LoadScene("MainStory");
    }
}
