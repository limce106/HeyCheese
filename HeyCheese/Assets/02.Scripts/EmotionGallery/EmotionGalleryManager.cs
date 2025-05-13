using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;

public class EmotionGalleryManager : MonoBehaviour
{
    [Header("UI 연결")]
    public Text titleText;
    public Dropdown filterDropdown;
    public GameObject detailPanel;
    public UnityEngine.UI.Image previewImage;
    public Text capturedAtText;
    public Text episodeTitleText;
    public Text selectedMoodText;
    public Transform contentParent;
    public GameObject thumbnailPrefab;

    private string dbPath;
    private List<GameObject> currentThumbnails = new List<GameObject>();

    void Start()
    {
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, "emotion_gallery.db");
        filterDropdown.onValueChanged.AddListener(OnFilterChanged);
        if (detailPanel != null)
            detailPanel.SetActive(false);
        LoadGallery("all");
    }

    public void LoadGallery(string filter)
    {
        ClearThumbnails();

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();

            string query = "SELECT * FROM emotion_gallery";
            if (filter == "story")
                query += " WHERE photo_type = 'story'";
            else if (filter == "free")
                query += " WHERE photo_type = 'free'";

            using (var cmd = new SqliteCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string path = reader["photo_path"].ToString();
                    string capturedAt = reader["captured_at"].ToString();
                    string photoType = reader["photo_type"].ToString();
                    string episodeTitle = reader["episode_title"].ToString();
                    string selectedMood = reader["selected_mood"].ToString();

                    GameObject item = Instantiate(thumbnailPrefab, contentParent);
                    currentThumbnails.Add(item);

                    // 썸네일 이미지 로드
                    UnityEngine.UI.Image img = item.transform.Find("ThumbnailImage").GetComponent<UnityEngine.UI.Image>();
                    if (File.Exists(path))
                    {
                        byte[] imgData = File.ReadAllBytes(path);
                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(imgData);
                        img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
                    }
                    else
                    {
                        Debug.LogWarning("이미지 파일 없음: " + path);
                    }

                    // 디테일 표시 이벤트 연결
                    Button btn = item.GetComponent<Button>();
                    btn.onClick.AddListener(() => ShowDetail(path, capturedAt, photoType, episodeTitle, selectedMood));
                }
            }
        }
    }

    void ClearThumbnails()
    {
        foreach (var thumb in currentThumbnails)
        {
            Destroy(thumb);
        }
        currentThumbnails.Clear();
    }

    void OnFilterChanged(int index)
    {
        string selected = filterDropdown.options[index].text;

        if (selected == "전체보기")
            LoadGallery("all");
        else if (selected == "스토리 사진")
            LoadGallery("story");
        else if (selected == "치즈한컷")
            LoadGallery("free");
    }

    void ShowDetail(string path, string capturedAt, string photoType, string episodeTitle, string selectedMood)
    {
        if (detailPanel == null) return;

        detailPanel.SetActive(true);

        if (File.Exists(path))
        {
            byte[] imgData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgData);
            previewImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }

        if (System.DateTime.TryParse(capturedAt, out var dt))
            capturedAtText.text = dt.ToString("yyyy-MM-dd HH:mm");
        else
            capturedAtText.text = capturedAt;

        if (photoType == "story")
        {
            episodeTitleText.gameObject.SetActive(true);
            selectedMoodText.gameObject.SetActive(true);
            episodeTitleText.text = "에피소드: " + (string.IsNullOrEmpty(episodeTitle) ? "-" : episodeTitle);
            selectedMoodText.text = "기록한 감정: " + (string.IsNullOrEmpty(selectedMood) ? "-" : selectedMood);
        }
        else
        {
            episodeTitleText.gameObject.SetActive(false);
            selectedMoodText.gameObject.SetActive(false);
        }
    }

    public void CloseDetail()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }
}

