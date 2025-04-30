using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;

public class DB_OutputTest : MonoBehaviour
{
    public UnityEngine.UI.Image previewImage;
    public Text capturedAtText;
    public Text episodeTitleText;
    public Text selectedMoodText;

    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/emotion_photo_test.db";
        LoadLatestPhoto();
    }

    void LoadLatestPhoto()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            string sql = "SELECT * FROM emotion_photo ORDER BY captured_at DESC LIMIT 1";

            using (var cmd = new SqliteCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string path = reader["photo_path"].ToString();
                    string capturedAt = reader["captured_at"].ToString();
                    string episodeTitle = reader["episode_title"].ToString();
                    string mood = reader["selected_mood"].ToString();

                    // 텍스트 표시
                    capturedAtText.text = "촬영일: " + capturedAt;
                    episodeTitleText.text = "에피소드: " + episodeTitle;
                    selectedMoodText.text = "기록한 감정: " + mood;

                    // 이미지 표시
                    if (File.Exists(path))
                    {
                        byte[] imgData = File.ReadAllBytes(path);
                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(imgData);
                        previewImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
                    }
                }
                else
                {
                    capturedAtText.text = "데이터가 없습니다.";
                }
            }
        }
    }
}
