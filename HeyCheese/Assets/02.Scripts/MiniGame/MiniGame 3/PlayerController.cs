using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 달리기 애니메이션")]
    public Animator PlayerAnimator;
    public RectTransform playerRect;
    public float burstDuration = 3f;
    public float burstSpeedMultiplier = 1.8f;
    public Vector2 burstOffset = new Vector2(150f, 0f);

    private bool hasStartedRunning = false;
    [SerializeField] private bool isInBurst = false;
    private float originalSpeed = 1f;
    private Vector2 originalPosition;

    private MiniGame3Manager miniGame3Manager;

    [Header("경쟁자들")]
    public RectTransform rival1;
    public RectTransform rival2;

    void Start()
    {
        miniGame3Manager = FindObjectOfType<MiniGame3Manager>();
        originalPosition = playerRect.anchoredPosition;

        // 경쟁자들 초기 딜레이 이동
        StartCoroutine(DelayAndMoveRival(rival1));
        StartCoroutine(DelayAndMoveRival(rival2));

        HandleTouch();
    }

    //void Update()
    //{
    //    // 클릭할 때 한 번만 실행해야 하기 때문에 
    //    // 아래와 같은 코드 사용
    //    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
    //    {
    //        HandleTouch();
    //    }
    //}

    // Touch Panel의 버튼 컴포넌트에 붙여서 사용함.
    public void HandleTouch()
    {
        if (!hasStartedRunning)
        {
            hasStartedRunning = true;
            PlayerAnimator.Play("player_runningStart");

            Invoke(nameof(StartRunningLoop), PlayerAnimator.GetCurrentAnimatorStateInfo(0).length);

            // 배경 스크롤 시작
            miniGame3Manager.scrollingBackground.SetIsRunning(true);
        }
        else
        {
            if (isInBurst) return; // 버스트 중이면 무시
            StartCoroutine(BurstSpeed());
        }
    }

    void StartRunningLoop()
    {
        PlayerAnimator.Play("player_running");
    }

    IEnumerator BurstSpeed()
    {
        isInBurst = true;

        SoundPlayer.Instance.SoundEffectPlay((int)SoundPlayer.SFX.Booster_SFX);

        // 버스트 시, 경쟁자 뒤쳐짐
        StartCoroutine(MoveRivalOnPlayerBurst());

        // 애니메이션 속도 증가
        originalSpeed = PlayerAnimator.speed;
        PlayerAnimator.speed *= burstSpeedMultiplier;

        // 배경 스크롤 속도 증가
        miniGame3Manager.scrollingBackground.SetBurstSpeed(true);
        miniGame3Manager.SetSliderSpeedMultiplier(burstSpeedMultiplier); // 슬라이더도 빨라짐

        // 위치 살짝 앞쪽으로
        StartCoroutine(MoveRect(playerRect, originalPosition + burstOffset,burstDuration));

        yield return new WaitForSeconds(burstDuration);

        // 원상 복구
        PlayerAnimator.speed = originalSpeed;

        // 배경 스크롤 속도 원상 복구
        miniGame3Manager.scrollingBackground.SetBurstSpeed(false);
        miniGame3Manager.SetSliderSpeedMultiplier(1f); // 원래 속도로

        StartCoroutine(MoveRect(playerRect, originalPosition, burstDuration));

        yield return new WaitForSeconds(burstDuration);

        isInBurst = false;
    }

    // 캐릭터 RectTransform  위치 부드럽게 이동
    IEnumerator MoveRect(RectTransform rect, Vector2 target, float duration)
    {
        float elapsedTime = 0;
        Vector2 start = rect.anchoredPosition;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rect.anchoredPosition = Vector2.Lerp(start, target, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = target;
    }

    // 골인 지점까지 오면 화면 밖으로 나가게 함.
    public void HidePlayerOffScreen()
    {
        StopAllCoroutines(); // 이동 중이면 멈춤
        Vector2 offScreenPos = new Vector2(2000f, playerRect.anchoredPosition.y);
        StartCoroutine(MoveRect(playerRect, offScreenPos, 0.5f));

        Vector2 offScreenPos_rival1 = new Vector2(1500f, rival1.anchoredPosition.y);
        Vector2 offScreenPos_rival2 = new Vector2(1500f, rival2.anchoredPosition.y);
        StartCoroutine(MoveRect(rival1, offScreenPos_rival1, 2f));
        StartCoroutine(MoveRect(rival2, offScreenPos_rival2, 2f));
    }

    // -------------------- 경쟁자 관련 메소드 ------------------------
    IEnumerator DelayAndMoveRival(RectTransform rival)
    {
        float delay = Random.Range(3f, 5f); // 3~5초 딜레이
        yield return new WaitForSeconds(delay);

        Vector2 targetPos = rival.anchoredPosition + new Vector2(-300f, 0f); // 플레이어보다 뒤쳐지게
        yield return StartCoroutine(MoveRect(rival, targetPos, 3f));
    }

    IEnumerator MoveRivalOnPlayerBurst()
    {
        float rightShift = Random.Range(50f, 100f);  // 오른쪽으로 밀림
        float leftWiggle = Random.Range(10f, 30f);  // 왼쪽으로 잠깐 치고 나옴

        RectTransform[] rivals = { rival1, rival2 };

        foreach (var rival in rivals)
        {
            Vector2 original = rival.anchoredPosition;
            Vector2 wiggleOut = original + new Vector2(leftWiggle, 0); // 살짝 앞으로
            Vector2 fallBack = original + new Vector2(-rightShift, 0);  // 더 뒤로

            // 앞으로 치고 나옴
            yield return StartCoroutine(MoveRect(rival, wiggleOut, 0.2f));

            // 다시 뒤로 밀림
            yield return StartCoroutine(MoveRect(rival, fallBack, 0.4f));
        }
    }
}
