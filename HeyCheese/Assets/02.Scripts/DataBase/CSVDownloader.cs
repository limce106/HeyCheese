using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

// Runtime 다운로드용 클래스
public class CSVDownloader : MonoBehaviour
{
    // 시트 이름과 아이디 딕셔너리 형태로 저장
    private static readonly Dictionary<string, string> sheetIds = new Dictionary<string, string>
    {
        {"MainStory", "1394039980"},
        {"StoryMenu", "248021801"}
    };

    private const string baseSheetURL = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&id={0}&gid={1}";
    private const string documentID = "1L19fvineZ5dGKvPFkx_PhsCvvLVg0ZMzUwFep7hTAP8";

    private void Start()
    {
        NetworkManager.Instance.CheckNetworkAndRun(() =>
        {
            //StartCoroutine(DownloadCSVs(() =>
            //{

            //}));
            DownloadCSVs();
        });
    }

    // CSV 다운로드 
    public static void DownloadCSVs(System.Action onComplete = null) //IEnumerator
    {
        foreach (var sheet in sheetIds)
        {
            string url = string.Format(baseSheetURL, documentID, sheet.Value);
            string filePath = Path.Combine(Application.persistentDataPath, $"{sheet.Key}.csv");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                //yield return request.SendWebRequest();
                var asyncOperation = request.SendWebRequest();
                while (!asyncOperation.isDone)
                    System.Threading.Thread.Sleep(100); // 동기적 대기

                if (request.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(filePath, request.downloadHandler.text);
                    Debug.Log($"Successfully downloaded and saved: {sheet.Key}.csv");
                }
                else
                    Debug.LogError($"Failed to download {sheet.Key}.csv: {request.error}");
            }
        }
        onComplete?.Invoke();
    }

    // CSV가 저장된 Path 리턴
    public static string GetCSVPath(string key)
    {
        return Path.Combine(Application.persistentDataPath, $"{key}.csv");
    }
}
