using UnityEngine;
using UnityEngine.UI;

public class CamDisplay : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private RawImage rawImage;
    private bool isPlaying = false;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

    void Update()
    {
        if (webcamTexture != null && webcamTexture.width > 16 && !isPlaying)
        {
            // 첫 프레임이 도착했으면 화면 업데이트
            rawImage.texture = webcamTexture;
            isPlaying = true;
        }
    }

    void OnDestroy()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}