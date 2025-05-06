using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스토리<->미니게임 시 해당 내용은 유지, 기억되어야 되지만
// 스토리 -> 목록 순간, 내용 초기화 필요
public class MainStoryManager : MonoBehaviour
{
    public static MainStoryManager Instance;
    public PlayerNameManager playerNameManager;
    public DialogueManager dialougeManager;
    //public Speaker speaker;

    [Header("Panels")]
    public GameObject loadingCanvas;
    public GameObject dialogueCanvas;
    public GameObject choiceCanvas;
    public GameObject inputFieldCanvas;
    public GameObject emotionCameraCanvas;
    public GameObject storyCameraCanvas;
    public GameObject giftCanvas;
    public GameObject settingsCanvas;

    [Header("UI Elements_Background")]
    public UnityEngine.UI.Image backgroundImg;
    [Header("UI Elements_Loading")] 
    public Button loadingBtn;
    public TMP_Text episodeIDText;
    public TMP_Text chapterTitleText;
    [Header("UI Elements_Dialogue")] // Dialogue
    public Button nextDialogueBtn;
    public UnityEngine.UI.Image characterImg;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public UnityEngine.UI.Image nameImg;
    public UnityEngine.UI.Image dialogueImg;
    public Slider bondSlider;
    [Header("UI Elements_Choice")] // Choice
    public TMP_Text choiceQuestionText;
    public Button choice1Btn;
    public TMP_Text choice1Text;
    public Button choice2Btn;
    public TMP_Text choice2Text;
    public Button choice3Btn;
    public TMP_Text choice3Text;
    [Header("UI Elements_InputField")] // InputField
    public TMP_Text inputNameText;
    [Header("UI Elements_Emotion Camera")] // Emotion Camera

    [Header("UI Elements_Story Camera")] // Story Camera

    [Header("UI Elements_Gift")] // Gift

    [Header("UI Elements_Setting")] // Setting



    // Data Structure
    public Dictionary<int, MainStory> currentEpisode; // 전달한 에피소드에 해당하는 {id:MainStory CSV의 한 열(step), ..}
    private int currentID = 0;
    public static string PlayerName = "주인공";

    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    //public void Clear()
    //{
    //    selectedLevel = 0;
    //    selectedEmotion = "None";
    //    selectedSituation = "None";
    //}

    //// 메인 씬 넘어갔을 때 호출 필요
    //public void DestroySelf()
    //{
    //    Clear();
    //    Destroy(gameObject);
    //    Instance = null;
    //}

    //// !!메인 씬 갔을 때 아래 작성해주기!!
    ////void Start()
    ////{
    ////    if (StoryManagerTest.Instance != null)
    ////    {
    ////        StoryManagerTest.Instance.DestroySelf();
    ////    }
    ////}

    void Start()
    {
        // 에피소드 메뉴에서 선택된 
        string episodeID = PlayerPrefs.GetString("SelectedEpisodeID");
        print("매니저: " + episodeID);

        // 파싱한 내용 불러오기
        CSVParser csvParser = new CSVParser();
        currentEpisode = csvParser.ParseMainStories(episodeID);
        Debug.Log($"[MainStoryManager] Loaded EpisodeID: {episodeID}");
        print(currentEpisode.Keys);
        print(currentEpisode.Values);
        currentID = 0;

        // 시작할 때 저장된 이름 불러오기
        LoadPlayerName();

        //loadingCanvas.gameObject.SetActive(true);
        ShowCurrentID(currentID);
    }

    //// 메뉴에서 버튼 눌렀을 시
    //public void OnStartStoryButtonClicked(string emotion, string situation, int level)
    //{
    //    selectedEmotion = emotion;
    //    selectedSituation = situation;
    //    selectedLevel = level;

    //    LoadStoryData();
    //    currentIndex = 0;
    //    menuCanvas.gameObject.SetActive(false);
    //    dialogueCanvas.gameObject.SetActive(true);
    //    ShowCurrentStep();
    //}

    //void LoadStoryData()
    //{
    //    currentStory.Clear();

    //    string path = Path.Combine(Application.streamingAssetsPath, "StoryDataTest.json");
    //    if (!File.Exists(path))
    //    {
    //        Debug.LogError("StoryDataTest.json 파일을 찾을 수 없습니다!");
    //        return;
    //    }

    //    string jsonString = File.ReadAllText(path);

    //    try
    //    {
    //        var allData = JsonHelperTest.FromJson<StoryDataTest>(jsonString);

    //        if (allData == null)
    //        {
    //            Debug.LogError("JSON 파싱 실패! allData가 null입니다.");
    //            return;
    //        }

    //        foreach (var item in allData)
    //        {
    //            if (item.EmotionID == selectedEmotion &&
    //                item.SituationID.StartsWith(selectedSituation) &&
    //                (item.Level == selectedLevel.ToString() || item.Level == "all"))
    //            {
    //                currentStory.Add(item);
    //            }
    //        }

    //        currentStory.Sort((a, b) => a.Order.CompareTo(b.Order));
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"JSON 파싱 중 예외 발생: {e.Message}");
    //    }
    //}

    // 현재 스텝(ID)에 맞는 이벤트 실행
    void ShowCurrentID(int id)
    {
        MainStory step = currentEpisode[id];
        currentID = id;

        if (step.NextID == 0)
        {
            Debug.Log("스토리 종료 지점에 도달했습니다.");
            EndEpisode();
            return;
        }

        // 잘못된 스텝 건너뛰기
        // id가 -1(문제o)인 경우, 해당 스텝(csv의 열)을 스킵함
        if (id == -1)
        {
            Debug.LogWarning($"[MainStory] ID 파싱 실패: 문제 스텝(열): {step}");
            NextStep();
        }

        switch (step.EventType)
        {
            case "Loading":
                print("Loading");
                TurnOffEveryCanvas();
                loadingCanvas.SetActive(true);

                SetImage(backgroundImg, step.ImageID); // 배경 변경
                
                episodeIDText.text = step.EpisodeID; // 에피소드ID 변경
                chapterTitleText.text = step.ChapterTitle; // 에피소드 제목 변경
                break;
            case "Video":
                print("Video 재생");
                break;
            case "Dialogue":
                TurnOffEveryCanvas();
                dialogueCanvas.SetActive(true);

                // 대사에 플레이어 이름 적용
                string rawScriptText = step.ScriptID;
                string namedScriptText = rawScriptText.Replace("{PlayerName}", PlayerName);

                speakerNameText.text = step.SpeakerID; // 이름 변경
                dialogueText.text = namedScriptText; // 대사 변경
                SetImage(backgroundImg, step.ImageID); // 배경 변경
                SetImage(characterImg, step.SpeakerImageID); // 캐릭터 변경

                // 이름,대사 배경 색 변경
                Speaker speaker = SpeakerUtil.ParseSpeakerID(step.SpeakerID); // speakerID 텍스트를 enum 타입으로 변환
                (Color nameColor, Color dialogueColor) = dialougeManager.GetColorBySpeaker(speaker); // 이름과 대사에 speaker별 컬러 적용
                nameImg.color = nameColor;
                dialogueImg.color = dialogueColor;
                Canvas.ForceUpdateCanvases(); // UI 업데이트 - 제거 시 정상적으로 색 반영되지 않음
                break;
            case "Choice":
                TurnOffEveryCanvas();
                choiceCanvas.SetActive(true);

                choiceQuestionText.text = step.ScriptID; // 대사 변경
                choice1Text.text = step.Choice1; // 선택지 변경
                choice2Text.text = step.Choice2; // 선택지 변경
                choice3Text.text = step.Choice3; // 선택지 변경

                break;
            case "NameInput":
                // 대사 버튼 막기

                inputFieldCanvas.SetActive(true);
                //playerNameManager.SavePlayerName();

                //PlayerName = PlayerNameManager.PlayerName; // 에피소드1의 경우 이 단계 이후 플레이어의 이름 업데이트


                // OnConfirmYes 시, 플레이어 이름 업데이트 및 다음 단계로 이동



                break;
            case "MiniGame":
                break;
            case "EmotionCamera":
                break;
            case "StoryCamera":
                break;
            case "Gift":
                break;
            //case "CameraOnly":
            //case "CameraARGuide":
            //    dialogueCanvas.gameObject.SetActive(false);
            //    cameraCanvas.gameObject.SetActive(true);
            //    break;
            //case "Result":
            //    dialogueText.text = "결과 화면입니다.";
            //    break;
        }
    }

    public void NextStep()
    {
        int nextID = currentEpisode[currentID].NextID;
        ShowCurrentID(nextID);
    }

    // 캔버스 전환을 위한 모든 캔버스 비활성화 함수
    // NameInput 시에는 캔버스 비활성화 하지 않음
    void TurnOffEveryCanvas()
    {
        loadingCanvas.SetActive(false);
        dialogueCanvas.SetActive(false);
        choiceCanvas.SetActive(false);
        inputFieldCanvas.SetActive(false);
        emotionCameraCanvas.SetActive(false);
        storyCameraCanvas.SetActive(false);
        giftCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }

    // 플레이어 이름 불러오기
    void LoadPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }
        else
        {
            PlayerName = "주인공";
        }
    }

    void SetImage(UnityEngine.UI.Image targetImg, string imgPath)
    {
        if (string.IsNullOrEmpty(imgPath)) // 사진 존재 x 시
        {
            return; // 사진 변경x
        }

        Sprite newSprite = Resources.Load<Sprite>($"Arts/{imgPath}");
        if (newSprite != null)
        {
            targetImg.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"[ImageChanger] 이미지 로드 실패: {imgPath}, 기본 이미지 사용");

            if (targetImg.name.Contains("background"))
            {
                SetDefaultBackground(targetImg);
            }
        }
    }

    void SetDefaultBackground(UnityEngine.UI.Image targetImg)
    {
        Sprite defaultSprite = Resources.Load<Sprite>($"Arts/2Back//defaultBackground");
        if (defaultSprite != null)
        {
            targetImg.sprite = defaultSprite;
        }
    }

    void EndEpisode()
    {

    }
}
