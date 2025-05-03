using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class CSVDownloaderEditor : EditorWindow
{
    // 시트 이름과 아이디 딕셔너리 형태로 저장
    private static readonly Dictionary<string, string> sheetIds = new Dictionary<string, string>
    {
        {"MainStory", "1394039980"},
        {"StoryMenu", "248021801"}
    };

    // Menu의 Tools/Download CSVs 클릭 시 CSV 파일 다운로드 됨
    [MenuItem("Tools/Download CSVs")]
    public static void DownloadCSVs()
    {
        foreach (var sheet in sheetIds)
        {
            string url = $"https://docs.google.com/spreadsheets/d/1L19fvineZ5dGKvPFkx_PhsCvvLVg0ZMzUwFep7hTAP8/export?format=csv&id=1L19fvineZ5dGKvPFkx_PhsCvvLVg0ZMzUwFep7hTAP8&gid={sheet.Value}";
            string filePath = Path.Combine(Application.dataPath, "Resources/Datas", $"{sheet.Key}.csv");

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var asyncOperation = webRequest.SendWebRequest();
                while (!asyncOperation.isDone)
                    System.Threading.Thread.Sleep(100); // 동기적 대기

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(filePath, webRequest.downloadHandler.text);
                    Debug.Log($"Successfully downloaded and saved: {sheet.Key}.csv");
                }
                else
                    Debug.LogError($"Failed to download {sheet.Key}.csv: {webRequest.error}");
            }
        }
        AssetDatabase.Refresh();
    }
}
