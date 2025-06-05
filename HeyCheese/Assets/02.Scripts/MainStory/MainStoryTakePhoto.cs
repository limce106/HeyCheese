using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Mono.Data.Sqlite;
using System.IO;

public class MainStoryTakePhoto : MonoBehaviour
{
    public MainStoryManager MainStoryManager;
    public EmotionGalleryDBWriter emotionGalleryDBWriter;
    public EmotionDetector emotionDetector;

    [SerializeField]
    private ARFaceManager arFaceManager;
    public SaveLoadPicture saveLoadPicture;

    // DB
    string dbPath;

    private void Start()
    {
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, "HeyCheese.db");
    }

    public void OnClick_EmotionCamera()
    {
        // 카메라 버튼 비활성화
        MainStoryManager.DeActivateCameraBtnInteraction();

        // 사진 캡쳐 및 저장
        StartCoroutine(saveLoadPicture.CaptureAndSave((filepath, capturedAt) =>
        {
            // 표정 탐색 패널 보여주기
            MainStoryManager.ShowFaceSearching();

            // 와이파이 연결 확인하며 감정 검출
            StartCoroutine(DetectWithWifiWatchdog(filepath, capturedAt));
        }));
    }

    public void OnClick_StoryCamera()
    {
        // 카메라 버튼 비활성화
        MainStoryManager.DeActivateCameraBtnInteraction();

        // 사진 캡쳐 및 저장
        StartCoroutine(saveLoadPicture.CaptureAndSave((filepath, capturedAt) =>
        {
            // 사진 저장
            MainStoryManager.SaveStoryPhoto(filepath, capturedAt);

            // episodeID, episodeTitle 가져오기
            //int currentID = MainStoryManager.CurrentID;
            //string episodeId = MainStoryManager.CurrentEpisode[currentID].EpisodeID;
            //string episodeTitle = MainStoryManager.CurrentEpisode[currentID].ChapterTitle;
            //string mood = "이건 나중에 수정할 예정:3";

            // DB에 저장
            //emotionGalleryDBWriter.InsertStoryPhoto(filepath, capturedAt, episodeId, episodeTitle, mood);

            // 다이얼로그 대사 갱신
            MainStoryManager.NextStep();
        }));
    }

    // 와이파이 연결 확인하며 사진 저장, 감정 검출
    // 사진 찍기 버튼을 눌러 사진을 저장한 후 감정 분석 중 Wi-Fi가 끊겼을 경우, 저장한 사진 삭제, 카메라 버튼 재활성화, 다시 찍을 수 있도록 유도
    private IEnumerator DetectWithWifiWatchdog(string savedFilePath, string capturedTime)
    {
        bool emotionDone = false; // 감정 검출 완료 여부
        Emotion resultEmotion = Emotion.Default;

        // 캡쳐된 사진을 통해 감정 검출
        IEnumerator detectRoutine = emotionDetector.DetectEmotionFromFile(savedFilePath, (emotion) =>
        {
            // 감정 검출 완료
            emotionDone = true;
            resultEmotion = emotion;
        });

        // 감정 검출 시작 코루틴(병렬처리)
        Coroutine runningDetectCoroutine = StartCoroutine(detectRoutine);

        // 감정 분석 도중 WiFi 끊겼는지 감시(병렬처리) ============================
        while (!emotionDone)
        {
            // 감정 검출 후 네트워크가 끊겼다면
            if (!NetworkManager.Instance.IsWifiConnected)
            {
                Debug.LogWarning("감정 분석 중 네트워크 끊김. 중단 후 재시도");
                StopCoroutine(runningDetectCoroutine); /// 감정 검출 코루틴 중지
                DeleteFileIfExists(savedFilePath); // 캡처 파일 삭제

                MainStoryManager.HideFaceSearching(); // 표정 탐색 패널 숨기기
                MainStoryManager.ActivateCameraBtnInteraction(); // 버튼 활성화
                yield break;
            }

            yield return null;
        }

        // 분석 완료 후 처리 =========================================================

        // 감정 분석 결과 저장
        string expression = resultEmotion.ToString(); // enum 타입 Emotion을 string으로 변환

        // episodeID, episodeTitle 가져오기
        int currentID = MainStoryManager.CurrentID;
        string episodeId = MainStoryManager.CurrentEpisode[currentID].EpisodeID;
        string episodeTitle = MainStoryManager.CurrentEpisode[currentID].ChapterTitle;

        // DB에 저장
        emotionGalleryDBWriter.InsertEmotionPhoto(savedFilePath, capturedTime, expression, episodeId, episodeTitle);

        // 표정 탐색 패널 숨기기
        MainStoryManager.HideFaceSearching();

        // 유대감 증가 및 다이얼로그 대사 갱신
        MainStoryManager.UpdateDialogueAndBond(resultEmotion);
    }

    // 사진 삭제
    public void DeleteFileIfExists(string savedFilePath)
    {
        if (string.IsNullOrEmpty(savedFilePath)) return;

        // 파일 먼저 삭제
        if (File.Exists(savedFilePath))
        {
            File.Delete(savedFilePath);
            Debug.Log("사진 삭제 완료: " + savedFilePath);
        }

        // SQLite DB에서 레코드 삭제
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            string query = "DELETE FROM emotion_gallery WHERE photo_path = @path";
            using (var cmd = new SqliteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@path", savedFilePath);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
