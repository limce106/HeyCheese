using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.SceneManagement;
using TMPro;

public class EmotionGalleryManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text titleText;
    public TMP_Dropdown filterDropdown;
    public GameObject detailPanel;
    public Image previewImage;
    public TMP_Text capturedAtText;
    public TMP_Text episodeTitleText;
    public TMP_Text selectedMoodText;
    public TMP_Text emotionTypeText;
    public Transform contentParent;
    public GameObject thumbnailPrefab;

    private string dbPath;
    private string currentSelectedPath;
    private List<GameObject> currentThumbnails = new List<GameObject>();

    void Start()
    {
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, "HeyCheese.db");
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
            else if (filter == "emotion")
                query += " WHERE photo_type = 'emotion'";

            query += " ORDER BY captured_at DESC";

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
                    string emotionType = reader["emotion_type"].ToString(); // ← 감정 사진용

                    GameObject item = Instantiate(thumbnailPrefab, contentParent);
                    currentThumbnails.Add(item);

                    Image img = item.transform.Find("ThumbnailImage").GetComponent<Image>();
                    if (File.Exists(path))
                    {
                        byte[] imgData = File.ReadAllBytes(path);
                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(imgData);
                        img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
                    }

                    Button btn = item.GetComponent<Button>();
                    btn.onClick.AddListener(() => ShowDetail(path, capturedAt, photoType, episodeTitle, selectedMood, emotionType));
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
        else if (selected == "감정 사진")
            LoadGallery("emotion");
    }


    public void OnClick_BackButton()
    {
        string previousScene = SceneHistoryManager.PreviousSceneName;

        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }



    // ----------------------------------- 아래는 디테일 패널 관련 메서드 -----------------------------------

    void ShowDetail(string path, string capturedAt, string photoType, string episodeTitle, string selectedMood, string emotionType)
    {
        if (detailPanel == null) return;

        currentSelectedPath = path;
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

        // 공통: episodeTitle은 스토리/감정 사진에서만 표시
        if (photoType == "story" || photoType == "emotion")
        {
            episodeTitleText.gameObject.SetActive(true);
            episodeTitleText.text = "에피소드: " + (string.IsNullOrEmpty(episodeTitle) ? "-" : episodeTitle);
        }
        else
        {
            episodeTitleText.gameObject.SetActive(false);
        }

        // 스토리 사진용 감정 텍스트
        if (photoType == "story")
        {
            selectedMoodText.gameObject.SetActive(true);
            emotionTypeText.gameObject.SetActive(false);
            selectedMoodText.text = "기록한 감정: " + (string.IsNullOrEmpty(selectedMood) ? "-" : selectedMood);
        }
        // 감정 사진용 감정 텍스트
        else if (photoType == "emotion")
        {
            selectedMoodText.gameObject.SetActive(false);
            emotionTypeText.gameObject.SetActive(true);
            emotionTypeText.text = "표현한 감정: " + (string.IsNullOrEmpty(emotionType) ? "-" : emotionType);
        }
        // 나머지(free)는 숨김
        else
        {
            selectedMoodText.gameObject.SetActive(false);
            emotionTypeText.gameObject.SetActive(false);
        }
    }



    public void CloseDetail()
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
            emotionTypeText.gameObject.SetActive(false);
        }
    }


    // 사진 삭제
    public void DeletePhoto()
    {
        if (string.IsNullOrEmpty(currentSelectedPath)) return;

        // 파일 먼저 삭제
        if (File.Exists(currentSelectedPath))
        {
            File.Delete(currentSelectedPath);
            Debug.Log("사진 삭제 완료: " + currentSelectedPath);
        }

        // SQLite DB에서 레코드 삭제
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            string query = "DELETE FROM emotion_gallery WHERE photo_path = @path";
            using (var cmd = new SqliteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@path", currentSelectedPath);
                cmd.ExecuteNonQuery();
            }
        }

        // 화면 갱신
        LoadGallery("all");
        detailPanel.SetActive(false);
    }


    // 사진 저장
    public void SaveToGallery()
    {
        if (string.IsNullOrEmpty(currentSelectedPath)) return;
        if (!File.Exists(currentSelectedPath)) return;

        byte[] imageData = File.ReadAllBytes(currentSelectedPath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageData);

        // 파일 이름 추출
        string filename = Path.GetFileName(currentSelectedPath);

        // HeyCheese라는 폴더로 저장됨
        NativeGallery.SaveImageToGallery(tex, "HeyCheese", filename);

        Debug.Log("갤러리에 저장 완료: " + filename);
    }

}

