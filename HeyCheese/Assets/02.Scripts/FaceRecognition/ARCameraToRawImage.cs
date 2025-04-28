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

            if (cameraTexture == null || cameraTexture.width != cpuImage.width || cameraTexture.height != cpuImage.height)
            {
                if (cameraTexture != null)
                    Destroy(cameraTexture);

                cameraTexture = new Texture2D(cpuImage.width, cpuImage.height, TextureFormat.RGBA32, false);
            }

            cpuImage.Convert(conversionParams, cameraTexture.GetRawTextureData<byte>());
            cameraTexture.Apply();

            // RawImage의 텍스처 업데이트
            rawImage.texture = cameraTexture;
            rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90);
            rawImage.rectTransform.sizeDelta = new Vector2(800, 800);
        }
    }
}