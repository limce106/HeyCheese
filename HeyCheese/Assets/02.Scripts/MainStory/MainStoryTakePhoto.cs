using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MainStoryTakePhoto : MonoBehaviour
{
    public MainStoryManager MainStoryManager;
    public EmotionGalleryDBWriter emotionGalleryDBWriter;
    public EmotionDetector emotionDetector;

    [SerializeField]
    private ARFaceManager arFaceManager;
    public SaveLoadPicture saveLoadPicture;

    public void OnClick_EmotionCamera()
    {
        MainStoryManager.emoCameraBtn.interactable =false; // 버튼 입력 막기

        // 사진 캡쳐 및 저장
        StartCoroutine(saveLoadPicture.CaptureAndSave((filepath, capturedAt) =>
        {
            // 캡쳐된 사진을 통해 감정 검출
            StartCoroutine(emotionDetector.DetectEmotionFromFile(filepath, (emotion) =>
            {
                // 감정 분석 결과 저장
                string expression = emotion.ToString(); // enum 타입 Emotion을 string으로 변환

                // episodeID, episodeTitle 가져오기
                int currentID = MainStoryManager.CurrentID;
                string episodeId = MainStoryManager.CurrentEpisode[currentID].EpisodeID;
                string episodeTitle = MainStoryManager.CurrentEpisode[currentID].ChapterTitle;

                // DB에 저장
                emotionGalleryDBWriter.InsertEmotionPhoto(filepath, capturedAt, expression, episodeId, episodeTitle);

                // 유대감 증가 및 다이얼로그 대사 갱신
                MainStoryManager.UpdateDialogueAndBond(emotion);
            }));
        }));
    }

    public void OnClick_StoryCamera()
    {
        // 사진 캡쳐 및 저장
        StartCoroutine(saveLoadPicture.CaptureAndSave((filepath, capturedAt) =>
        {
            // episodeID, episodeTitle 가져오기
            int currentID = MainStoryManager.CurrentID;
            string episodeId = MainStoryManager.CurrentEpisode[currentID].EpisodeID;
            string episodeTitle = MainStoryManager.CurrentEpisode[currentID].ChapterTitle;
            string mood = "이건 나중에 수정할 예정:3";

            // DB에 저장
            emotionGalleryDBWriter.InsertStoryPhoto(filepath, capturedAt, episodeId, episodeTitle, mood);

            // 다이얼로그 대사 갱신
            MainStoryManager.NextStep();
        }));
    }
}
