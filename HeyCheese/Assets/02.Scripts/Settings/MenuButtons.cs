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

    #region Settings
    // 모드별 환경 설정 버튼 활성화 여부 결정
    // Menu에 있을 시 제작자(MadeBy), 부모님 가이드(ParentGuide)만 활성화
    // Menu가 아닐 시 메인 메뉴로(toMainMenu)만 활성화
    public void SetMode(SettingsMode mode)
    {
        bool isMenu = (mode == SettingsMode.Menu);

        // 버튼 활성화/비활성화 설정
        toMainMenuBtn.SetActive(!isMenu);
        madeByBtn.SetActive(isMenu);
        parentGuideBtn.SetActive(isMenu);
    }

    // 환경 설정 버튼 클릭 시 환경 설정 화면 띄움
    public void OnClickSettingsOpen()
    {
        // 씬 이름에 따른 환경 설정 속 버튼 활성화 여부 결정
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu" || currentScene == "EpisodeMenu")
        {
            SetMode(SettingsMode.Menu);
        }
        else
        {
            SetMode(SettingsMode.InGame);
        }

        // 환경 설정 패널 열기
        settingWindow.SetActive(true);
    }

    public void OnClickSettingsClose()
    {
        settingWindow.SetActive(false);
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
