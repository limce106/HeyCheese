using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame3Manager : MiniGameManager
{
    [SerializeField] private Slider goalSlider;
    private float sliderSpeedMultiplier = 1f; // 기본값은 1
    [SerializeField] private float duration = 30f;
    private float elapsedTime = 0f;

    public bool isPlaying = false;

    [SerializeField] private TextMeshProUGUI countdownText;
    private readonly string[] countdownValues = { "3", "2", "1", "시작!" };

    public ScrollingBackground scrollingBackground;

    private new void Awake()
    {
        base.Awake();

        Time.timeScale = 0f; // 게임 정지
        goalSlider.minValue = 0;
        goalSlider.maxValue = duration;
    }


    // " 3, 2, 1, 시작! " 카운트 다운 표시

    public override void StartGame()
    {
        GuidePanel.SetActive(false);
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            Time.timeScale = 0f; // 게임 정지
            StartCoroutine(StartCountDown());
        }
    }


    private IEnumerator StartCountDown()
    {
        foreach (var value in countdownValues)
        {
            countdownText.text = value;
            yield return new WaitForSecondsRealtime(1);
        }

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1f; // 게임 시작

        StartGoalSlider(); // 슬라이더 시작
        isPlaying = true;
    }

    private void OnGoalComplete()
    {
        scrollingBackground.SetIsRunning(false);
        isPlaying = false;

        // 플레이어를 왼쪽으로 밀기
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.HidePlayerOffScreen();
        }

        // 클리어 패널 표시
        ClearPanel.SetActive(true);

        // 2초 후 자동으로 메인스토리 이동
        StartCoroutine(BackToMainStory());
    }

    // ************************* temporary methods for goal slider *************************
    private void StartGoalSlider()
    {
        StartCoroutine(RunGoalSlider());
    }

    // 플레이어 터치해서 버스트 중일 때 이 메소드로 더 빨리 슬라이더 차오르게 함.
    public void SetSliderSpeedMultiplier(float multiplier)
    {
        sliderSpeedMultiplier = multiplier;
    }

    // 15초 동안 진행되는 슬라이더
    private IEnumerator RunGoalSlider()
    {
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * sliderSpeedMultiplier;

            if (goalSlider != null)
                goalSlider.value = Mathf.Clamp(elapsedTime, 0, duration);

            yield return null;
        }

        if (goalSlider != null)
            goalSlider.value = duration;

        OnGoalComplete(); // 완주 처리
    }
}
