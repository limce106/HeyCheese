using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SettingsMode
{
    Menu,
    InGame
}

public class MenuButtons : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static MenuButtons Instance { get; private set; }

    [SerializeField]
    public GameObject settingWindow;

    [Header("Panel Setting")]
    // Slider
    public Slider ttsSlider;
    public Slider sfxSlider;
    public Slider bgmSlider;
    // Button
    public GameObject toMainMenuBtn;
    public GameObject madeByBtn;
    public GameObject parentGuideBtn;

    [Header("Panel ParentGuide")]
    public GameObject parentGuideGO;

    [Header("Panel MadeBy")]
    public GameObject madeByGO;

    private void Awake()
    {
        // 싱글톤 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
    }


    #region Settings
    private void OnEnable()
    {
        // 씬 전환 이벤트 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 이벤트 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 바뀔 때 자동 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateButtonsByScene(scene.name);
    }

    // 씬 이름에 따른 환경 설정 속 버튼 활성화 여부 결정
    private void UpdateButtonsByScene(string sceneName)
    {
        if (sceneName == "MainMenu" || sceneName == "EpisodeMenu")
        {
            SetMode(SettingsMode.Menu);
        }
        else
        {
            SetMode(SettingsMode.InGame);
        }
    }

    // 모드에 따라 버튼 활성화 설정
    // Menu에 있을 시 제작자(MadeBy), 부모님 가이드(ParentGuide)만 활성화
    // Menu가 아닐 시 메인 메뉴로(toMainMenu)만 활성화
    public void SetMode(SettingsMode mode)
    {
        bool isMenu = (mode == SettingsMode.Menu);

        // 버튼 활성화/비활성화 설정
        toMainMenuBtn?.SetActive(!isMenu);
        madeByBtn?.SetActive(isMenu);
        parentGuideBtn?.SetActive(isMenu);
    }

    // 환경 설정 버튼 클릭 시 환경 설정 화면 띄움
    public void OnClickSettingsOpen()
    {
        // 안전하게 다시 확인 (sceneLoaded와 중복 가능)
        string currentScene = SceneManager.GetActiveScene().name;
        UpdateButtonsByScene(currentScene);

        // 환경 설정 패널 열기
        settingWindow.SetActive(true);

        // 시간 일시정지
        Time.timeScale = 0f;
    }

    // 설정창 닫기
    public void OnClickSettingsClose()
    {
        settingWindow.SetActive(false);
        parentGuideGO.SetActive(false);
        madeByGO.SetActive(false);

        // 시간 일시정지 해제
        Time.timeScale = 1f;
    }
    #endregion

    #region PlayerPrefs
    public void OnClickCheckPlayerPrefs()
    {
        PlayerPrefsControll.PrintAllPrefs(); // 저장 확인용
    }
    public void OnClickDeletePlayerPrefs_EpisodeUnLock()
    {
        PlayerPrefsControll.DeleteEpisodeUnLock();
    }
    #endregion
}
