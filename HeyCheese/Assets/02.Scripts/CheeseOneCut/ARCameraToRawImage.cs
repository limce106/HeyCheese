using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

[RequireComponent(typeof(ARCameraManager))]
public class ARCameraToRawImage : MonoBehaviour
{
    public RawImage rawImage;

    private ARCameraManager cameraManager;
    private Texture2D cameraTexture;
    private Texture2D rotatedTexture;
    private NativeArray<byte> rawPixelBuffer;

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

        if (rawPixelBuffer.IsCreated)
            rawPixelBuffer.Dispose();

        if (cameraTexture != null)
            Destroy(cameraTexture);

        if (rotatedTexture != null)
            Destroy(rotatedTexture);
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        using (cpuImage)
        {
            int width = cpuImage.width;
            int height = cpuImage.height;
            int bufferSize = width * height * 4;

            if (!rawPixelBuffer.IsCreated || rawPixelBuffer.Length != bufferSize)
            {
                if (rawPixelBuffer.IsCreated)
                    rawPixelBuffer.Dispose();

                rawPixelBuffer = new NativeArray<byte>(bufferSize, Allocator.Persistent);
            }

            if (cameraTexture == null || cameraTexture.width != width || cameraTexture.height != height)
            {
                if (cameraTexture != null)
                    Destroy(cameraTexture);
                if (rotatedTexture != null)
                    Destroy(rotatedTexture);

                cameraTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                rotatedTexture = new Texture2D(height, width, TextureFormat.RGBA32, false);
            }

            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, width, height),
                outputDimensions = new Vector2Int(width, height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };

            cpuImage.Convert(conversionParams, rawPixelBuffer);
            cameraTexture.LoadRawTextureData(rawPixelBuffer);
            cameraTexture.Apply();

            Rotate90(cameraTexture, rotatedTexture);

            rawImage.texture = rotatedTexture;

            int rawImageWidth = Screen.width;
            int rawImageHeight = (int)(Screen.width * 0.75);
            rawImage.rectTransform.sizeDelta = new Vector2(rawImageWidth, rawImageHeight);
        }
    }

    private void Rotate90(Texture2D source, Texture2D dest)
    {
        Color32[] srcPixels = source.GetPixels32();
        Color32[] destPixels = new Color32[srcPixels.Length];

        int srcWidth = source.width;
        int srcHeight = source.height;

        for (int y = 0; y < srcHeight; ++y)
        {
            for (int x = 0; x < srcWidth; ++x)
            {
                int rotatedX = y;
                int rotatedY = srcWidth - 1 - x;
                destPixels[rotatedY * srcHeight + rotatedX] = srcPixels[y * srcWidth + x];
            }
        }

        dest.SetPixels32(destPixels);
        dest.Apply();
    }
}
