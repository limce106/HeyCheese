using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections;
using UnityEngine.Android;

public class DBInputTest : MonoBehaviour
{
    [Header("UI 구성")]
    public RawImage webcamDisplay;
    public Image frameImage;
    public Button storyCaptureButton;
    public Button freeCaptureButton;

    [Header("팝업 입력창 (story용)")]
    public GameObject popupWindow;
    public Dropdown dropdownEpisode;
    public Dropdown dropdownExpression;
    public Dropdown dropdownMood;
    public Button confirmButton;

    private WebCamTexture webcamTexture;
    private string latestCapturedPath;
    private string latestCapturedAt;

    private IDbConnection dbConnection;

    void Start()
    {
        // 카메라 권한 요청
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        // 웹캠 시작
        webcamTexture = new WebCamTexture();
        webcamDisplay.texture = webcamTexture;
        webcamTexture.Play();

        // 버튼 클릭 이벤트 연결
        storyCaptureButton.onClick.AddListener(() => StartCoroutine(CaptureAndSave("story")));
        freeCaptureButton.onClick.AddListener(() => StartCoroutine(CaptureAndSave("free")));
        confirmButton.onClick.AddListener(OnConfirmPopup);

        // 팝업창 처음에는 꺼두기
        popupWindow.SetActive(false);

        // DB 연결만 (테이블 생성 X)
        OpenDB("emotion_gallery.db");
    }

    //  DB 연결
    void OpenDB(string dbName)
    {
        string path = Path.Combine(Application.persistentDataPath, dbName);
        dbConnection = new SqliteConnection("URI=file:" + path);
        dbConnection.Open();
        Debug.Log("DB 연결 완료: " + path);
    }

    // 사진 캡처하고 저장
    IEnumerator CaptureAndSave(string photoType)
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        // 파일명, 경로 설정
        string filename = "emotion_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filepath = Path.Combine(Application.persistentDataPath, filename);

        File.WriteAllBytes(filepath, screenImage.EncodeToPNG());
        Destroy(screenImage);

        latestCapturedPath = filepath;
        latestCapturedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        if (photoType == "story")
        {
            popupWindow.SetActive(true); // story면 팝업 열기
        }
        else
        {
            InsertFreePhoto(filepath, latestCapturedAt);
        }
    }

    // 자유사진 저장
    void InsertFreePhoto(string filepath, string capturedAt)
    {
        string query = $@"
        INSERT OR REPLACE INTO emotion_gallery (
            photo_path, captured_at, photo_type
        ) VALUES (
            '{filepath}', '{capturedAt}', 'free'
        );";

        ExecuteNonQuery(query);
        Debug.Log("자유 사진 저장 완료!");
    }

    // 스토리 사진 저장
    void OnConfirmPopup()
    {
        string episodeTitle = dropdownEpisode.options[dropdownEpisode.value].text;
        int episodeId = dropdownEpisode.value + 1; // 드롭다운 인덱스 기준
        string expression = dropdownExpression.options[dropdownExpression.value].text;
        string mood = dropdownMood.options[dropdownMood.value].text;

        string query = $@"
        INSERT OR REPLACE INTO emotion_gallery (
            photo_path, captured_at, photo_type,
            emotion_type, episode_id, episode_title, selected_mood
        ) VALUES (
            '{latestCapturedPath}', '{latestCapturedAt}', 'story',
            '{expression}', {episodeId}, '{episodeTitle}', '{mood}'
        );";

        ExecuteNonQuery(query);
        Debug.Log("스토리 사진 저장 완료!");
        popupWindow.SetActive(false);
    }

    // 쿼리 실행 (Insert 용)
    void ExecuteNonQuery(string query)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();
    }

    // 종료 시 DB 연결 닫기
    void OnDestroy()
    {
        dbConnection?.Close();
    }
}
