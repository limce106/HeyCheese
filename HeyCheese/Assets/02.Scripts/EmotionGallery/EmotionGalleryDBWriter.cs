using System;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public class EmotionGalleryDBWriter : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, "HeyCheese.db");
        EnsureTableExists(); // 테이블 없으면 생성
    }

    private void EnsureTableExists()
    {
        using var conn = new SqliteConnection(dbPath);
        conn.Open();
        using var cmd = conn.CreateCommand();

        // emotion_gallery 테이블 생성 (존재하면 무시)
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS emotion_gallery (
                photo_path TEXT PRIMARY KEY,
                captured_at TEXT NOT NULL,
                photo_type TEXT NOT NULL,
                emotion_type TEXT,
                episode_id TEXT,
                episode_title TEXT,
                selected_mood TEXT
            );";
        cmd.ExecuteNonQuery();
        Debug.Log("[DB] emotion_gallery 테이블 확인 및 생성 완료");
    }

    // 치즈한컷 DB 저장
    public void InsertFreePhoto(string filepath, string capturedAt)
    {
        using var conn = new SqliteConnection(dbPath);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
            INSERT OR REPLACE INTO emotion_gallery (
                photo_path, captured_at, photo_type
            ) VALUES (
                '{filepath}', '{capturedAt}', 'free'
            );";
        cmd.ExecuteNonQuery();
        Debug.Log("[DB] 치즈한컷 저장 완료: " + filepath);
    }

    // 스토리 사진 DB 저장
    public void InsertStoryPhoto(string filepath, string capturedAt, string episodeId, string episodeTitle, string mood)
    {
        using var conn = new SqliteConnection(dbPath);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
            INSERT OR REPLACE INTO emotion_gallery (
                photo_path, captured_at, photo_type,
                episode_id, episode_title, selected_mood
            ) VALUES (
                '{filepath}', '{capturedAt}', 'story',
                '{episodeId}', '{episodeTitle}', '{mood}'
            );";
        cmd.ExecuteNonQuery();
        Debug.Log("[DB] 스토리 사진 저장 완료: " + filepath);
    }

    // 감정 사진 DB 저장
    public void InsertEmotionPhoto(string filepath, string capturedAt, string expression, string episodeId, string episodeTitle)
    {
        using var conn = new SqliteConnection(dbPath);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
            INSERT OR REPLACE INTO emotion_gallery (
                photo_path, captured_at, photo_type,
                emotion_type, episode_id, episode_title
            ) VALUES (
                '{filepath}', '{capturedAt}', 'emotion',
                '{expression}', '{episodeId}', '{episodeTitle}'
            );";
        cmd.ExecuteNonQuery();
        Debug.Log("[DB] 감정 사진 저장 완료: " + filepath);
    }
}
