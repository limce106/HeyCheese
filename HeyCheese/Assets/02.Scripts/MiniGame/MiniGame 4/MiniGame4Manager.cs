using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame4Manager : MiniGameManager
{
    public GameObject[] characterObjects; // 캐릭터 3개 등록
    //private Animator[] animators;
    public GameObject Background_Sleeping;

    [SerializeField] private Slider progressSlider;

    [Header("진행바 핸들 이미지")]
    [SerializeField] private Image barHandleImage;
    // 터치 안 하면 878787 헥사 코드로 변경 

    public float fillDuration = 10f; // 10초에 다 채워짐
    public float decayRate = 0.05f;   // 초당 감소 비율

    [Header("Adult 이미지 | Sprite 리스트 0:LightOn, 1:LightOff")]
    [SerializeField] private Image AdultImage;
    [SerializeField] private List<Sprite> AdultSprites;

    // 어른 랜덤으로 등장(효과음 재생 이후 2초 후에 등장)
    [Header("Adult 랜덤으로 등장할 확률")]
    [SerializeField] private int apprearanceProbability = 50;
    public bool isAdultComing = false;
    private bool isTouching;
    private bool previousTouchState; // 이전 터치 상태 저장
    private bool isCleared = false;

    private bool isStart;
    public bool isRestart = false;

    [SerializeField] private GameObject RestartPanel;

    private enum currentState
    {
        SLEEPING,
        DANCING
    }


    private new void Awake()
    {
        base.Awake();

        Time.timeScale = 0f;

        isStart = false;
        isAdultComing = false;
        previousTouchState = false; // 초기화

        //animators = new Animator[characterObjects.Length];
        for (int i = 0; i < characterObjects.Length; i++)
        {
            //animators[i] = characterObjects[i].GetComponent<Animator>();
            characterObjects[i].SetActive(false); // 시작 시 비활성화
        }

        SetRestartActive(false);
        SetAdultImage(false);

        // 자장가 bgm 재생
        SoundPlayer.Instance.ChangeBGM((int)SoundPlayer.BGM.MINIGAME4_BGM_SLEEPING);
    }

    void Update()
    {
        // 시작 아직 안 했거나 클리어 한 상태면 터치 안 받음
        if (!isStart || isCleared) return;

        //// 게임 시작된 상태면 터치를 받아서 isTouching true로 변경
        //CheckTouchInput();

        // 터치 상태에 따라 캐릭터 상태 변경
        HandleTouchEffects();
        // 터치 상태에 따라 진행 슬라이더 상태 변경
        UpdateProgressSlider();
        // Clear 조건 달성 체크
        CheckClearCondition();

        // 게임 진행 중일 때만 체크
        if (isStart && !isCleared && isAdultComing && isTouching)
        {
            TriggerRestart();
        }
    }

    // Touch Panel에서 PointerDown일 때
    public void OnTouchDown()
    {
        if (!isStart || isCleared) return;
        isTouching = true;
    }

    // Touch Panel에서 PointerUp일 때
    public void OnTouchUp()
    {
        isTouching = false;
    }


    private void TriggerRestart()
    {
        if (isRestart) return; // 이미 처리된 경우 중복 방지

        isRestart = true;

        SoundPlayer.Instance.SoundEffectPlay((int)SoundPlayer.SFX.TimeOut_SFX);

        SetRestartActive(true);

        StopAllCoroutines();
    }

    public override void StartGame()
    {
        if (isRestart)
        {
            StopAllCoroutines();
            InitMiniGame4();
        }

        GuidePanel.SetActive(false);
        SetRestartActive(false);

        Time.timeScale = 1f;
        isStart = true;
        isRestart = false;
        previousTouchState = false; // 게임 시작 시 초기화

        StartCoroutine(CheckAdultAppearanceLoop());
    }

    // 터치 감지 메소드
    // 지속적인 입력 처리
    private void CheckTouchInput()
    {
#if UNITY_EDITOR
        isTouching = Input.GetMouseButton(0);
#else
    Touch[] touches = Input.touches;
    isTouching = touches != null && touches.Length > 0;
#endif
    }

    private void HandleTouchEffects()
    {
        // 터치 상태가 변경되었을 때만 BGM 변경
        if (isTouching != previousTouchState)
        {
            if (isTouching)
            {
                // 터치 시작: 댄싱 BGM으로 변경
                SoundPlayer.Instance.ChangeBGM((int)SoundPlayer.BGM.MINIGAME4_BGM_DANCING);
            }
            else
            {
                // 터치 종료: 자장가 BGM으로 변경
                SoundPlayer.Instance.ChangeBGM((int)SoundPlayer.BGM.MINIGAME4_BGM_SLEEPING);
            }

            previousTouchState = isTouching; // 현재 상태를 이전 상태로 저장
        }

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

        if (isAdultComing)
            SetAdultImage(true);
    }

    private void UpdateProgressSlider()
    {
        // 터치 중이면 슬라이더 증가
        if (isTouching)
        {
            progressSlider.value += Time.deltaTime / fillDuration;

            // 핸들 이미지도 밝게 처리
            barHandleImage.color = HexColor("#FFFFFF");
        }
        // 터치 안 할 때, 슬라이더 서서히 감소함
        else
        {
            progressSlider.value -= Time.deltaTime * decayRate;

            // 슬라이더바의 핸들 이미지도 어둡게 처리
            barHandleImage.color = HexColor("#878787");

        }

        // value가 0~1 사이에서만 유지되도록 제한
        progressSlider.value = Mathf.Clamp01(progressSlider.value);
    }

    // 헥사값 컬러 반환( 코드 순서 : RGBA )
    private Color HexColor(string hexCode)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexCode, out color))
        {
            return color;
        }

        Debug.LogError("[UnityExtension::HexColor]invalid hex code - " + hexCode);
        return Color.white;
    }


    private void CheckClearCondition()
    {
        // 조건 달성 시 게임 시간 멈추고 ClearPanel 표시
        if (progressSlider.value >= 1f)
        {
            SoundPlayer.Instance.SoundEffectPlay((int)SoundPlayer.SFX.Shouting_SFX);

            isCleared = true;
            Time.timeScale = 0f;

            if (ClearPanel != null)
                ClearPanel.SetActive(true);

            // 2초 후 자동으로 메인스토리 이동
            StartCoroutine(BackToMainStory());
        }
    }


    void ActivateCharacters(bool activate)
    {
        for (int i = 0; i < characterObjects.Length; i++)
        {
            if (characterObjects[i].activeSelf != activate)
            {
                characterObjects[i].SetActive(activate);
            }

            //if (activate)
            //    animators[i].SetBool("IsDancing", true);
            //else
            //    animators[i].SetBool("IsDancing", false);
        }
    }

    private void SetRestartActive(bool isShow)
    {
        RestartPanel.SetActive(isShow);
        Time.timeScale = 0f;
        isStart = false;
    }

    private void InitMiniGame4()
    {
        progressSlider.value = 0f;

        GuidePanel.SetActive(false);
        ClearPanel.SetActive(false);
        isAdultComing = false;
        SetAdultImage(false);
        previousTouchState = false; // 초기화

        // 자장가 bgm 재생
        SoundPlayer.Instance.ChangeBGM((int)SoundPlayer.BGM.MINIGAME4_BGM_SLEEPING);
    }


    private IEnumerator CheckAdultAppearanceLoop()
    {
        while (true)
        {
            yield return StartCoroutine(AppearRandomly());

            // 다음 시도를 일정 시간 후에 다시
            yield return new WaitForSeconds(Random.Range(3f, 10f));
        }
    }

    private IEnumerator AppearRandomly()
    {
        int randomNumber = Random.Range(0, 100);
        float waitingTime = 3f;

        if (randomNumber < apprearanceProbability)
        {
            // 노크 소리 재생
            SoundPlayer.Instance.SoundEffectPlay((int)SoundPlayer.SFX.KNOCKING_SFX);

            yield return new WaitForSeconds(waitingTime);
            // 문 여는 효과음 재생
            SoundPlayer.Instance.SoundEffectPlay((int)SoundPlayer.SFX.OPENDOOR_SFX);

            // 효과음 다 재생된 후 등장
            yield return new WaitWhile(()=>SoundPlayer.Instance.isSoundEffectPlaying());

            isAdultComing = true;
            SetAdultImage(true);

            // 일정 시간 후 사라지기
            yield return new WaitForSeconds(waitingTime);

            // 문 닫는 효과음 재생
            SoundPlayer.Instance.SoundEffectPlay((int)SoundPlayer.SFX.CLOSEDOOR_SFX);

            // 효과음 다 재생된 후 사라짐
            yield return new WaitWhile(() => SoundPlayer.Instance.isSoundEffectPlaying());

            isAdultComing = false;
            SetAdultImage(false);
        }
    }



    private void SetAdultImage(bool isActive)
    {
        if (isActive&& isAdultComing)
        {
            AdultImage.gameObject.SetActive(true);

            // 어른 왔을 때 터치하고 있으면 불켜져 있는 상태 스프라이트인 0번째로 변경
            AdultImage.sprite = AdultSprites[
                (isTouching==true ? 0 : 1)
                ];
        }
        else
        {
            AdultImage.gameObject.SetActive(false);
        }

    }
}
