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
    private NativeArray<byte> rawPixelBuffer;
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
    }

    private Texture2D Rotate90(Texture2D originalTexture)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;

        // 회전된 텍스처의 크기는 원본 텍스처의 너비와 높이를 바꾼 값
        Texture2D rotatedTexture = new Texture2D(height, width, originalTexture.format, false);

        // 원본 텍스처의 색상 데이터를 가져온다.
        Color32[] originalPixels = originalTexture.GetPixels32();

        // 회전된 텍스처의 색상 배열을 준비합니다
        Color32[] rotatedPixels = new Color32[originalPixels.Length];

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                // (x, y)를 (rotatedX, rotatedY)로 변환하여 색상 데이터를 복사
                int rotatedX = y;
                int rotatedY = width - 1 - x;

                rotatedPixels[rotatedY * height + rotatedX] = originalPixels[y * width + x];
            }
        }

        // 회전된 색상 데이터를 새로운 텍스처에 적용
        rotatedTexture.SetPixels32(rotatedPixels);
        rotatedTexture.Apply();

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

            if (bufferSize != currentBufferSize)
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
        }
    }
}
