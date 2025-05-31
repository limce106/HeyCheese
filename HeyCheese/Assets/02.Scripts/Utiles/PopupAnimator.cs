using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PopupAnimator
{
    public enum EaseType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad
    }

    // ease 적용 
    private static float ApplyEase(float t, EaseType ease)
    {
        switch (ease)
        {
            case EaseType.EaseInQuad: // 느리게 -> 빠르게
                return t * t;
            case EaseType.EaseOutQuad: // 빠르게 -> 느리게
                return 1 - (1 - t) * (1 - t);
            case EaseType.EaseInOutQuad: // 천천히 -> 빠르게 -> 천천히
                return t < 0.5f
                    ? 2f * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
            default:
                return t; // Linear
        }
    }

    // 애니메이션: 페이드 인 + 슬라이드 다운
    private static IEnumerator AnimationPopupIn(GameObject targetPanel, float moveY, float duration, EaseType ease = EaseType.EaseInQuad)
    {
        CanvasGroup canvasGroup = targetPanel.GetComponent<CanvasGroup>();
        RectTransform rect = targetPanel.GetComponent<RectTransform>();

        Vector2 startPos = rect.anchoredPosition; // 부모 안에서 UI 요소가 얼마나 떨어져있는지
        Vector2 endPos = startPos + new Vector2(0f, -moveY); // y축으로 moveY 이동

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration); // 시간에 따라 변화하는 값을 0~1로 정규화
            float eased = ApplyEase(normalized, ease);

            canvasGroup.alpha = eased; // 페이드 인
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased); // 슬라이드 다운

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rect.anchoredPosition = endPos;
    }
    // 애니메이션: 페이드 아웃 + 슬라이드 업
    private static IEnumerator AnimationPopupOut(GameObject targetPanel, float moveY, float duration, EaseType ease = EaseType.EaseOutQuad)
    {
        CanvasGroup canvasGroup = targetPanel.GetComponent<CanvasGroup>();
        RectTransform rect = targetPanel.GetComponent<RectTransform>();

        Vector2 startPos = rect.anchoredPosition; // 부모 안에서 UI 요소가 얼마나 떨어져있는지
        Vector2 endPos = startPos + new Vector2(0f, moveY); // y축으로 moveY 이동

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration); // 시간에 따라 변화하는 값을 0~1로 정규화
            float eased = ApplyEase(normalized, ease);

            canvasGroup.alpha = 1f - eased; // 페이드 아웃
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased); // 슬라이드 업

            yield return null;
        }

        canvasGroup.alpha = 0f;
        rect.anchoredPosition = endPos;
    }

    // 패널 팝업
    public static IEnumerator OnPanelPopup(GameObject targetPanel, float moveY = 150f, float duration = 0.5f)
    {
        CanvasGroup canvasGroup = targetPanel.GetComponent<CanvasGroup>();

        Debug.Log("실행");
        canvasGroup.alpha = 0f;
        targetPanel.SetActive(true); // 패널 표시
        yield return AnimationPopupIn(targetPanel, moveY, duration); // 0 > 1
        yield return new WaitForSeconds(3f);
        yield return AnimationPopupOut(targetPanel, moveY, duration); // 1 > 0
        targetPanel.SetActive(false); // 패널 숨기기
        canvasGroup.alpha = 0f;
    }
}
