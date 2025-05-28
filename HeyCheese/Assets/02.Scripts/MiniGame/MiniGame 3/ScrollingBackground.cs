using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    private bool isRunning = false;

    // 배경 이동 속도 조절 (1~20 사이 값 설정 가능)
    [SerializeField] [Range(1f, 20f)] public float speed = 3f;
    // 처음 시작은 3f 스피드였다가
    // 5f로 스피드 값 올라가고 고정.
    // 화면 연속으로 터치하면 7f정도로 올라갔다가 다시 5f로 돌아옴.

    public RawImage rawImage;
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.1f, 0);

    private Rect uvRect;

    void Update()
    {
        if (isRunning)
        {
            uvRect = rawImage.uvRect;
            uvRect.x += scrollSpeed.x * Time.deltaTime * speed; // 오른쪽 스크롤
            //uvRect.y += scrollSpeed.y * Time.deltaTime;
            rawImage.uvRect = uvRect;
        }
        
    }

    public void SetIsRunning(bool isRunning)
    {
        this.isRunning = isRunning;
    }

    public bool GetIsRunning()
    {
        return this.isRunning;
    }

    public void SetBurstSpeed(bool isBurst)
    {
        if(isBurst)
            speed = 8f;
        else
            speed = 5f;
    }


}