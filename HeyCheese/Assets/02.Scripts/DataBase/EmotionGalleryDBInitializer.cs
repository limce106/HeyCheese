using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class EmotionGalleryDBInitializer : MonoBehaviour
{
    private IDbConnection dbConnection; // DB 연결 객체

    void Start()
    {
        // 실행 시 DB 열기
        OpenDB("emotion_gallery.db");
        // 테이블 생성
        CreateTables();
    }

    /// <summary>
    /// DB 파일을 열고 연결
    /// </summary>
    void OpenDB(string dbName)
    {
        string path = Path.Combine(Application.persistentDataPath, dbName);
        dbConnection = new SqliteConnection("URI=file:" + path);
        dbConnection.Open();
        Debug.Log("DB 연결 완료: " + path);
    }

    /// <summary>
    /// emotion_gallery 테이블을 생성 (이미 존재하면 넘어감)
    /// </summary>
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
        Debug.Log("테이블 생성 완료");
    }

    /// <summary>
    /// 쿼리를 실행하는 공용 함수
    /// </summary>
    void ExecuteNonQuery(string query)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();
    }

    void OnDestroy()
    {
        // 스크립트가 종료될 때 연결 종료
        dbConnection?.Close();
    }
}
