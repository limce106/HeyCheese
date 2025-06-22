using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class EnvLoader
{
    private static bool loaded = false;

    /// <summary>
    /// 안드로이드 환경에서는 StreamingAssets 내 파일을 UnityWebRequest로 읽어야 함
    /// </summary>
    public static IEnumerator LoadEnvCoroutine(Action onComplete = null)
    {
        if (loaded)
        {
            onComplete?.Invoke();
            yield break;
        }

        string path = Path.Combine(Application.streamingAssetsPath, ".env");
        string content = null;

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            content = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError("EnvLoader: Failed to load .env from StreamingAssets: " + request.error);
            yield break;
        }
#else
        // 에디터 및 PC 환경은 그냥 파일 읽기 가능
        if (File.Exists(path))
        {
            content = File.ReadAllText(path);
        }
        else
        {
            Debug.LogWarning("EnvLoader: .env file not found at path: " + path);
            yield break;
        }
#endif

        // 파싱해서 환경변수 설정
        SetEnvVariables(content);

        loaded = true;
        onComplete?.Invoke();
    }

    private static void SetEnvVariables(string content)
    {
        if (string.IsNullOrEmpty(content)) return;

        string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        foreach (var line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;

            string[] parts = trimmed.Split(new[] { '=' }, 2);
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                // 환경 변수로 설정 (프로세스 내에서만 유효)
                Environment.SetEnvironmentVariable(key, value);
                Debug.Log($"EnvLoader: Loaded key '{key}'");
            }
        }
    }
}
