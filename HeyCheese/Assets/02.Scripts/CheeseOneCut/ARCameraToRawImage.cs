using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[RequireComponent(typeof(ARCameraManager))]
public class ARCameraToRawImage : MonoBehaviour
{
    public RawImage rawImage;

    private ARCameraManager cameraManager;
    private Texture2D cameraTexture;
    private NativeArray<byte> rawPixelBuffer;
    private NativeArray<Color32> colorPixelBuffer;
    private int bufferSize;

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
        if (colorPixelBuffer.IsCreated)
            colorPixelBuffer.Dispose();
    }

    private Texture2D Rotate90(Texture2D originalTexture)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;
        Texture2D rotatedTexture = new Texture2D(height, width, originalTexture.format, false);

        NativeArray<Color32> originalPixels = originalTexture.GetRawTextureData<Color32>();
        NativeArray<Color32> rotatedPixels = rotatedTexture.GetRawTextureData<Color32>();

        int originalStride = width;
        int rotatedStride = height;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                rotatedPixels[x * rotatedStride + (rotatedStride - 1 - y)] = originalPixels[y * originalStride + x];
            }
        }

        rotatedTexture.Apply();
        originalPixels.Dispose();
        rotatedPixels.Dispose();
        return rotatedTexture;
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

            int currentBufferSize = cpuImage.width * cpuImage.height * 4;

            if (cameraTexture == null || cameraTexture.width != cpuImage.width || cameraTexture.height != cpuImage.height)
            {
                if (cameraTexture != null)
                    Destroy(cameraTexture);

                cameraTexture = new Texture2D(cpuImage.width, cpuImage.height, TextureFormat.RGBA32, false);
                bufferSize = currentBufferSize;
                if (rawPixelBuffer.IsCreated)
                    rawPixelBuffer.Dispose();
                rawPixelBuffer = new NativeArray<byte>(bufferSize, Allocator.Persistent);
            }
            else if (bufferSize != currentBufferSize)
            {
                bufferSize = currentBufferSize;
                if (rawPixelBuffer.IsCreated)
                    rawPixelBuffer.Dispose();
                rawPixelBuffer = new NativeArray<byte>(bufferSize, Allocator.Persistent);
            }

            cpuImage.Convert(conversionParams, rawPixelBuffer);
            cameraTexture.LoadRawTextureData(rawPixelBuffer);
            cameraTexture.Apply();

            // 이미지 회전
            cameraTexture = Rotate90(cameraTexture);

            rawImage.texture = cameraTexture;
            rawImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, 180);
        }
    }
}