using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame1Manager : MonoBehaviour
{
    enum CharacterIconName
    {
        Cheese,
        Bugi
    }

    [SerializeField] private Slider timeoutSlider;
    private float duration = 30f;
    private float elapsedTime = 0f;

    public bool isPlaying = false;

    public bool isRestart = false;

    public int currentStageLevel = 0;
    public int maxStageLevel = 2;

    [Header("숨바꼭질 스테이지")]
    [SerializeField] private List<GameObject> gameStagesList;
    [Header("숨은 캐릭터들")]
    [SerializeField] private List<HiddenCharacterButton> hiddenCharactersList;

    [SerializeField] private int findOutScore = 0;

    [SerializeField] private List<Image> characterIconsList;

    [Header("패널들")]

    [SerializeField] private GameObject GuidePanel;
    [SerializeField] private TextMeshProUGUI GuideLevelText;

    [SerializeField] private GameObject RestartPanel;
    [SerializeField] private TextMeshProUGUI RestartLevelText;

    [SerializeField] private GameObject ClearPanel;

    private void Awake()
    {
        isRestart = false;

        currentStageLevel = 0;
        findOutScore = 0;
        InitTimeSlider();

        SetGuideActive(true);
    }


    public void StartHideAndSeek()
    {
        isPlaying = true;

        findOutScore = 0;
        InitCharacterIcon();

        SetGameStage(currentStageLevel);

        StartTimer();

        // 켜져 있는 가이드 or 재시작 패널들 끔
        SetGuideActive(false);
        SetRestartActive(false);

        isRestart = false;
    }

    private void SetGuideActive(bool isShow)
    {
        if (isShow)
        {
            GuidePanel.SetActive(isShow);

            switch (currentStageLevel)
            {
                case 0:
                    GuideLevelText.text = "✨첫 번째 숨바꼭질!✨";
                    break;
                case 1:
                    GuideLevelText.text = "✨두 번째 숨바꼭질!✨";
                    break;
            }
        }
        else
        {
            GuidePanel.SetActive(isShow);
        }
        
    }

    private void SetRestartActive(bool isShow)
    {
        if (!isRestart)
            return;

        if (isShow)
        {
            RestartPanel.SetActive(isShow);

            switch (currentStageLevel)
            {
                case 0:
                    RestartLevelText.text = "✨첫 번째 숨바꼭질!✨";
                    break;
                case 1:
                    RestartLevelText.text = "✨두 번째 숨바꼭질!✨";
                    break;
            }
        }
        else
        {
            RestartPanel.SetActive(isShow);
        }

    }


    private void SetGameStage(int currentStageLevel)
    {
        foreach (GameObject stage in gameStagesList)
        {
            stage.SetActive(false);
        }

        // 재시작이면 숨은 캐릭터 버튼 상태 초기화
        if (isRestart)
        {
            foreach (HiddenCharacterButton hiddenCharacterButton in hiddenCharactersList)
            {
                hiddenCharacterButton.initHiddenState();
            }
        }

        switch (currentStageLevel)
        {
            case 0:
                gameStagesList[currentStageLevel].SetActive(true);
                break;

            case 1:
                gameStagesList[currentStageLevel].SetActive(true);
                break;

            default:
                break;

        }

    }

    public void IncrementFindOutScore()
    {
        ++findOutScore;

        StartCoroutine(checkFindOutScore());
    }

    private IEnumerator checkFindOutScore()
    {
        if (findOutScore == 2)
        {
            yield return new WaitForSeconds(1.5f);

            currentStageLevel = currentStageLevel <= maxStageLevel ?
            ++currentStageLevel : maxStageLevel;

            // 숨바꼭질 모든 단계 클리어
            if(currentStageLevel == maxStageLevel)
            {
                // 숨바꼭질 끝!
                ClearPanel.SetActive(true);
                yield break;
            }

            SetGuideActive(true);
        }
        else
        {
            yield return null;
        }

    }


    // 치즈, 부기 찾은 진행도 표시
    public void SetCharacterIcon(int characterName)
    {
        switch(characterName)
        {
            case (int)CharacterIconName.Cheese:
                Color color = characterIconsList[(int)CharacterIconName.Cheese].color;
                color.a = 1f;
                characterIconsList[(int)CharacterIconName.Cheese].color = color;
                break;

            case (int)CharacterIconName.Bugi:
                color = characterIconsList[(int)CharacterIconName.Bugi].color;
                color.a = 1f;
                characterIconsList[(int)CharacterIconName.Bugi].color = color;
                break;
        }
    }

    private void InitCharacterIcon()
    {
        foreach(Image iconImage in characterIconsList)
        {
            Color color = iconImage.color;
            color.a = 0.4f;
            iconImage.color = color;
        }
    }

    // ************************* temporary methods for time slider *************************
    private void StartTimer()
    {
        InitTimeSlider();
        StartCoroutine(RunTimer());
    }

    // 30초 흘러가는 걸 보여주는 타이머
    private IEnumerator RunTimer()
    {
        elapsedTime = 0f;

        while (elapsedTime<duration)
        {
            elapsedTime += Time.deltaTime;

            if (timeoutSlider != null)
                timeoutSlider.value = Mathf.Clamp(elapsedTime, 0, duration);

            if (findOutScore == 2)
                yield break;

            yield return null;
        }

        if (timeoutSlider != null)
            timeoutSlider.value = duration;

        // 제한 시간 동안 다 못 찾으면
        if(findOutScore<2)
        {
            // restart 패널 켜짐
            isRestart = true;
            SetRestartActive(true);
        }

        isPlaying = false;
    }

    // 슬라이더 관련 변수 초기화
    private void InitTimeSlider()
    {
        timeoutSlider.minValue = 0f;
        timeoutSlider.maxValue = duration;
        timeoutSlider.value = 0f;
    }
}
