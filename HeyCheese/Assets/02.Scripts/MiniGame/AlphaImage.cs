using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaImage : MonoBehaviour
{
    // 투명화된 png 파일의 불투명한 부분만 터치되게 만들음.
    // 해당 기능 사용하려면 
    // 이미지를 Advanced에 있는 Read/Write Enabled를 true로 바꿔 줘야 함.

    public float AlphaThreshold = 0.1f;

    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
    }
}