using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame4Manager : MonoBehaviour
{
    public GameObject[] characterObjects; // 캐릭터 3개 등록
    //private Animator[] animators;
    public GameObject Background_Sleeping;

    [SerializeField] private Slider progressSlider;

    public float fillDuration = 15f; // 15초에 다 채워짐
    public float decayRate = 0.1f;   // 초당 감소 비율

    private bool isTouching;
    private bool isCleared = false;

    [Header("패널들")]
    [SerializeField] private GameObject GuidePanel;
    [SerializeField] private GameObject ClearPanel;

    private bool isStart;

    private void Awake()
    {
        Time.timeScale = 0f;

        isStart = false;

        //animators = new Animator[characterObjects.Length];
        for (int i = 0; i < characterObjects.Length; i++)
        {
            //animators[i] = characterObjects[i].GetComponent<Animator>();
            characterObjects[i].SetActive(false); // 시작 시 비활성화
        }

        GuidePanel.SetActive(true);
        ClearPanel.SetActive(false);
    }

    void Update()
    {
        // 시작 아직 안 했거나 클리어 한 상태면 터치 안 받음
        if (!isStart || isCleared) return;

        // 게임 시작된 상태면 터치를 받아서 isTouching true로 변경
        CheckTouchInput();

        // 터치 상태에 따라 캐릭터 상태 변경
        HandleTouchEffects();
        // 터치 상태에 따라 진행 슬라이더 상태 변경
        UpdateProgressSlider();
        // Clear 조건 달성 체크
        CheckClearCondition();
    }

    public void StartGame()
    {
        GuidePanel.SetActive(false);
        Time.timeScale = 1f;
        isStart = true;
    }

    // 터치 감지 메소드
    // 지속적인 입력 처리
    private void CheckTouchInput()
    {
#if UNITY_EDITOR
        isTouching = Input.GetMouseButton(0); // 에디터용
#else
    isTouching = Input.touchCount > 0;    // 모바일 터치 감지
#endif
    }

    private void HandleTouchEffects()
    {
        if (isTouching)
        {
            // 터치 중일 때, 캐릭터들 춤을 춤
            ActivateCharacters(true);
            Background_Sleeping.SetActive(false);
        }
        else
        {
            // 터치 안 하면, 춤 멈추고 자는 척 함.
            ActivateCharacters(false);
            Background_Sleeping.SetActive(true);
        }
    }

    private void UpdateProgressSlider()
    {
        // 터치 중이면 슬라이더 증가
        if (isTouching)
        {
            progressSlider.value += Time.deltaTime / fillDuration;
        }
        // 터치 안 할 때, 슬라이더 서서히 감소함
        else
        {
            progressSlider.value -= Time.deltaTime * decayRate;
        }

        // value가 0~1 사이에서만 유지되도록 제한
        progressSlider.value = Mathf.Clamp01(progressSlider.value);
    }

    private void CheckClearCondition()
    {
        // 조건 달성 시 게임 시간 멈추고 ClearPanel 표시
        if (progressSlider.value >= 1f)
        {
            isCleared = true;
            Time.timeScale = 0f;

            if (ClearPanel != null)
                ClearPanel.SetActive(true);
        }
    }


    void ActivateCharacters(bool activate)
    {
        for (int i = 0; i < characterObjects.Length; i++)
        {
            if (characterObjects[i].activeSelf != activate)
                characterObjects[i].SetActive(activate);

            //if (activate)
            //    animators[i].SetBool("IsDancing", true);
            //else
            //    animators[i].SetBool("IsDancing", false);
        }
    }
}
