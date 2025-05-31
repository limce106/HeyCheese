using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// non-lazy, DDOL 싱글톤 게임 매니저
// MainStory 씬에서 발생
// 스토리 <-> 미니게임 전환 시에도 ID, bondScore 등 저장하는 역할
// MainStory, MiniGame 이외의 씬에 가면 Destroy
// 현재 스토리 진행 상태 보관
public class MainStoryGameManager : MonoBehaviour
{
    #region Singleton
    public static MainStoryGameManager MainStoryGM;
    #endregion

    public Dictionary<int, MainStory> currentEpisode; // 전달한 에피소드에 해당하는 {id:MainStory CSV의 한 열(step), ..}
    public int currentID = 0; // 현재 ID를 저장하여 스토리<->미니게임 전환 시에도 이어서 스토리를 열람할 수 있도록 한다.
    public string playerName = "주인공";


    #region Unity Lifecycle
    private void Awake()
    {
        if (MainStoryGM != null && MainStoryGM != this)
        {
            Destroy(gameObject); // 중복 방지
        }
        else
        {
            MainStoryGM = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 전환 감지

        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 리스너 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름 확인
        if (!IsAllowedScene(scene.name))
        {
            Debug.Log($"[MainStoryGM] {scene.name}은 허용된 씬이 아니므로 파괴됩니다.");
            Destroy(gameObject);
            MainStoryGM = null;
        }
    }
    // 살아있어야 하는 씬
    private bool IsAllowedScene(string sceneName)
    {
        return sceneName.StartsWith("MainStory") || sceneName.StartsWith("MiniGame");
    }

    void Start()
    {
        // 에피소드 메뉴에서 선택된 에피소드ID 가져오기
        string episodeID = PlayerPrefs.GetString("SelectedEpisodeID");

        // 파싱한 내용 불러오기
        CSVParser csvParser = new CSVParser();
        currentEpisode = csvParser.ParseMainStories(episodeID);
        Debug.Log($"[MainStoryManager] Loaded EpisodeID: {episodeID}");

        // 시작할 때 저장된 이름 불러오기
        PlayerDataManager.Instance.LoadPlayerName(); // 이름 불러오기
        playerName = PlayerDataManager.Instance.PlayerName; // 이름 가져오기
    }
    #endregion

    // 미니게임->스토리 시 다음 이벤트로 넘어갈 수 있도록 currentID 변경
    public void NextStep()
    {
        int nextID = currentEpisode[currentID].NextID;
        currentID = nextID;
    }
}
