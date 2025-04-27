using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARCameraManager))]
public class ARCameraToRawImage : MonoBehaviour
{
    public RawImage rawImage;

    private ARCameraManager cameraManager;
    private Texture2D cameraTexture;

    void Awake()
    {
        cameraManager = GetComponent<ARCameraManager>();
        cameraManager.requestedFacingDirection = CameraFacingDirection.User;
    }

    void OnEnable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        using (cpuImage)
        {
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };

            // Texture가 없거나 사이즈가 다르면 새로 생성
            if (cameraTexture == null || cameraTexture.width != cpuImage.width || cameraTexture.height != cpuImage.height)
            {
                if (cameraTexture != null)
                    Destroy(cameraTexture);

                cameraTexture = new Texture2D(cpuImage.width, cpuImage.height, TextureFormat.RGBA32, false);
                rawImage.texture = cameraTexture;
                rawImage.rectTransform.sizeDelta = new Vector2(cpuImage.width, cpuImage.height);
            }

            // CPU 이미지 -> Texture 변환
            cpuImage.Convert(conversionParams, cameraTexture.GetRawTextureData<byte>());
            cameraTexture.Apply();
        }
    }
}
