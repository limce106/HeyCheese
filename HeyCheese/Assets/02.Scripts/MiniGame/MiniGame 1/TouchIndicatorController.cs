using UnityEngine;
using UnityEngine.UI;


public class TouchIndicatorController : MonoBehaviour
{
    // 한 손가락으로 터치하고 있는 위치 표시하는 컨트롤러

    public RectTransform touchIndicator; // 원 이미지 (UI Image)로 터치 위치 표시

    void Start()
    {
        if (touchIndicator != null)
            touchIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
#if UNITY_EDITOR
        // 마우스 테스트용
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition;
            ShowIndicator(pos);
        }
        else
        {
            HideIndicator();
        }
#else
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            // 화면을 터치 시작/터치 중일 때 화면에 터치 위치 표시
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                ShowIndicator(touchPos);
            }
            // 화면 터치 안 하면 터치 위치 표시 이미지 끄기
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                HideIndicator();
            }
        }
        else
        {
            HideIndicator();
        }
#endif
    }

    void ShowIndicator(Vector2 screenPos)
    {
        if (touchIndicator != null)
        {
            // 이미지 위치에 화면 좌표를 넣어서 이동시킴
            touchIndicator.gameObject.SetActive(true);
            touchIndicator.position = screenPos;
        }
    }

    void HideIndicator()
    {
        if (touchIndicator != null && touchIndicator.gameObject.activeSelf)
        {
            touchIndicator.gameObject.SetActive(false);
        }
    }
}
