using UnityEngine;
using UnityEngine.UI;

public class EmotionDisplay : MonoBehaviour
{
    public Text resultText;  // 표정을 표시할 UI 텍스트

    // 표정 감지 후 텍스트로 출력하는 함수
    public void DisplayEmotion(string emotion)
    {
        // 표정을 텍스트로 표시
        resultText.text = "표정: " + emotion;
    }
}
