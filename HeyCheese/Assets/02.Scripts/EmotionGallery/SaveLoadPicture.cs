using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using System;
using UnityEngine.UI;

public class SaveLoadPicture : MonoBehaviour
{
    [Header("UI")]
    public RectTransform panelToCapture;

    // 사진 경로
    private string latestCapturedPath;
    // 찍은 날짜
    private string latestCapturedAt;

    private IDbConnection dbConnection;

    void Start()
    {
        //OpenDB("emotion_photo_test.db");
        OpenDB("emotion_gallery.db");
        CreateTables();
    }
      
    void OpenDB(string dbName)
    {
        string path = Path.Combine(Application.persistentDataPath, dbName);
        dbConnection = new SqliteConnection("URI=file:" + path);
        dbConnection.Open();
        Debug.Log("DB 연결: " + path);
    }

    void CreateTables()
    {
        string query = @"
        CREATE TABLE IF NOT EXISTS emotion_gallery (
            photo_path TEXT PRIMARY KEY,       -- 사진 파일 경로
            captured_at TEXT NOT NULL,          -- 촬영 일시
            photo_type TEXT NOT NULL,           -- 사진 유형 (story/free)
            emotion_type TEXT,                  -- 인식된 감정 (스토리 사진만 해당)
            episode_id INTEGER,                 -- 에피소드 ID (스토리 사진만 해당)
            episode_title TEXT,                 -- 에피소드 제목 (스토리 사진만 해당)
            selected_mood TEXT                  -- 기록한 감정 내용 (스토리 사진만 해당)
        );
        ";
        ExecuteNonQuery(query);
    }

    public IEnumerator CaptureAndSave()
    {
        yield return new WaitForEndOfFrame();

        // 캡처할 UI 패널의 화면 좌표 계산
        Vector3[] corners = new Vector3[4];
        panelToCapture.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;
        
        // 화면 좌표를 픽셀 좌표로 변환
        x = Mathf.Clamp(x, 0, Screen.width);
        y = Mathf.Clamp(y,0, Screen.height);
        width = Mathf.Clamp(width, 0, Screen.width - x);
        height = Mathf.Clamp(height, 0, Screen.height - y);

        // y 좌표는 아래에서 위로 계산되므로 보정 필요
        y = Screen.height - y - height;

        Texture2D screenImage = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(x, y, width, height), 0, 0);
        screenImage.Apply();

        string filename = "emotion_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filepath = Path.Combine(Application.persistentDataPath, filename);

        File.WriteAllBytes(filepath, screenImage.EncodeToPNG());
        Destroy(screenImage);

        latestCapturedPath = filepath;
        latestCapturedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        InsertFreePhoto(filepath, latestCapturedAt);
    }

    void InsertFreePhoto(string filepath, string capturedAt)
    {
        string query = $@"
        INSERT OR REPLACE INTO emotion_gallery (
            photo_path, captured_at, photo_type
        ) VALUES (
            '{filepath}', '{capturedAt}', 'free'
        );";

        ExecuteNonQuery(query);
        Debug.Log("치즈 한 컷 저장 완료");
    }

    void ExecuteNonQuery(string query)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();
    }

    void OnDestroy()
    {
        dbConnection?.Close();
    }


}
