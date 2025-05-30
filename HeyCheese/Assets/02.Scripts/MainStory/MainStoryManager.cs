using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

// UI에 이벤트 표시 역할
// 스토리<->미니게임 시 해당 내용은 유지, 기억되어야 되지만
// 스토리 -> 목록 순간, 내용 초기화 필요
public class MainStoryManager : MonoBehaviour
{
    #region Singleton
    //public static MainStoryManager MainStoryManager;
    //public PlayerNameManager playerNameManager;
    public DialogueManager dialougeManager;
    //public Speaker speaker;
    public ARFaceFilterApplier arFaceFilterApplier;
    public FrameApplier frameApplier;
    public EmotionGalleryDBWriter emotionGalleryDBWriter;
    #endregion

    #region UI References
    [Header("Panels")]
    public GameObject loadingCanvas;
    public GameObject dialogueCanvas;
    public GameObject choiceCanvas;
    public GameObject inputFieldCanvas;
    public GameObject cameraCanvas;
    public GameObject emotionCameraCanvas;
    public GameObject storyCameraCanvas;
    public GameObject giftCanvas;
    public GameObject settingsCanvas;

    [Header("UI Elements_Background")]
    public GameObject background;
    public Image backgroundImg;
    [Header("UI Elements_Loading")] 
    public Button loadingBtn;
    public TMP_Text episodeIDText;
    public TMP_Text chapterTitleText;
    [Header("UI Elements_Dialogue")] // Dialogue
    public Button nextDialogueBtn;
    public UnityEngine.UI.Image characterImg;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public GameObject nameImgObj;
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
    public TMP_Text emoQuestionText;
    public Button emoCameraBtn;
    [Header("UI Elements_Story Camera")] // Story Camera
    public Button arCameraBtn;
    [Header("UI Elements_FaceSearching")] // Camera_FaceSearching
    public GameObject faceSearchingPanel;
    [Header("UI Elements_Gift")] // Gift
    public GameObject giftAlarmPanel;
    public TMP_Text giftInfoText;
    [Header("UI Elements_Setting")] // Setting

    [Header("Camera")]
    public Camera arCamera;
    public Camera storyCamera;
    public ARSession arSession;

    // StoryCameraPhoto Data
    private string storyPhoto_filepath;
    private string storyPhoto_capturedAt;
    private string storyPhoto_mood;

    // Filter and Frame
    private string filterName = "";
    private string frameName = "";

    #endregion

    #region Data
    // Data Structure
    public Dictionary<int, MainStory> CurrentEpisode; // 전달한 에피소드에 해당하는 {id:MainStory CSV의 한 열(step), ..}
    public int CurrentID
    {
        get => MainStoryGameManager.MainStoryGM.currentID;
        set => MainStoryGameManager.MainStoryGM.currentID = value;
    }
    public float BondScore // 실제로 저장되는 BondScore
    {
        get => BondScoreDataManager.Instance.BondScore;
        set => BondScoreDataManager.Instance.BondScore = value;
    }
    [SerializeField] private float episodeBondScore = 0f; // 중단 시 점수 초기화 관리를 위한 BondScore
    public string PlayerName;
    #endregion

    #region Unity Lifecycle
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

    //void Start()
    //{
    //    // 에피소드 메뉴에서 선택된 
    //    string episodeID = PlayerPrefs.GetString("SelectedEpisodeID");
    //    print("매니저: " + episodeID);

    //    // 시작할 때 저장된 이름 불러오기
    //    //LoadPlayerName();

    //    //loadingCanvas.gameObject.SetActive(true);
    //    ShowCurrentID(CurrentID);
    //}

    //public GameObject imageObject; // 활성화 여부를 감지할 Image GameObject
    //private bool wasActive = false;
    //private void Update()
    //{
    //    if (imageObject.activeSelf)
    //    {
    //        wasActive = true;
    //        OnImageActivated();
    //    }
    //    else if (!imageObject.activeSelf)
    //    {
    //        wasActive = false;
    //        OnImageDeactivated();
    //    }
    //}

    //void OnImageActivated()
    //{
    //    Debug.Log("이미지가 활성화되었습니다!");
    //    // 여기에 활성화 시 실행할 코드 작성
    //}

    //void OnImageDeactivated()
    //{
    //    Debug.Log("이미지가 비활성화되었습니다!");
    //    // 여기에 비활성화 시 실행할 코드 작성
    //}


    // 코루틴으로 MainStoryGameManager.cs에서 csv 파싱 완료될 때까지 기다리기
    IEnumerator Start() 
    {
        yield return new WaitUntil(() => MainStoryGameManager.MainStoryGM?.currentEpisode != null);

        CurrentEpisode = MainStoryGameManager.MainStoryGM.currentEpisode;
        PlayerName = MainStoryGameManager.MainStoryGM.playerName;

        // 스토리카메라 데이터 내역 초기화
        ResetStoryPhotoData();        

        // 유대감 슬라이드 내역 반영
        bondSlider.value = BondScore;
        episodeBondScore = BondScore;

        ShowARView();
        ShowCurrentID(CurrentID);
    }
    #endregion

    #region Story Logic
    // 현재 스텝(ID)에 맞는 이벤트 실행
    void ShowCurrentID(int id)
    {
        MainStory step = CurrentEpisode[id];
        CurrentID = id;

        // 잘못된 스텝 건너뛰기
        // id가 -1(문제o)인 경우, 해당 스텝(csv의 열)을 스킵함
        if (id == -1)
        {
            Debug.LogWarning($"[MainStory] ID 파싱 실패: 문제 스텝(열): {step}");
            NextStep();
        }

        background.SetActive(true);
        ShowStoryView();
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

                if(SpeakerUtil.ParseSpeakerID(step.SpeakerID) == Speaker.System)
                {
                    nameImgObj.SetActive(false);
                }
                else
                {
                    nameImgObj.SetActive(true);
                }
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
            case "SaveChoice":
                TurnOffEveryCanvas();
                choiceCanvas.SetActive(true);

                choiceQuestionText.text = step.ScriptID; // 대사 변경
                choice1Text.text = step.Choice1; // 선택지 변경
                choice2Text.text = step.Choice2; // 선택지 변경
                choice3Text.text = step.Choice3; // 선택지 변경

                break;
            case "NameInput":
                nextDialogueBtn.interactable = false; // 대사 버튼 막기
                inputFieldCanvas.SetActive(true);

                // 이후 이름 인풋, 저장은 PlayerNameManager.cs가 처리
                //playerNameManager.SavePlayerName();
                //PlayerName = PlayerNameManager.PlayerName; // 에피소드1의 경우 이 단계 이후 플레이어의 이름 업데이트
                break;
            case "MiniGame":
                // 미니게임 씬으로 이동
                SceneManager.LoadScene(step.MiniGame);
                break;
            case "EmotionCamera":
                ShowARView();
                TurnOffEveryCanvas();
                ActivateCameraBtnInteraction();
                background.SetActive(false);
                cameraCanvas.SetActive(true);
                emotionCameraCanvas.SetActive(true);
                
                // 질문에 플레이어 이름 적용
                string rawQuestionText = "치즈가 {PlayerName}의 얼굴을 보는 중\n표정으로 내 감정을 알려주자.";
                emoQuestionText.text = rawQuestionText.Replace("{PlayerName}", PlayerName);
                break;
            case "StoryCamera":
                ShowARView();
                TurnOffEveryCanvas();
                ActivateCameraBtnInteraction();
                background.SetActive(false);
                cameraCanvas.SetActive(true);
                storyCameraCanvas.SetActive(true);
                
                // 필터 추가
                filterName = ConvertEpisodeToFilterName(step.EpisodeID);
                arFaceFilterApplier.MainStory_Filter(filterName);
                // 프레임 추가
                frameName = ConvertEpisodeToFrameName(step.EpisodeID);
                frameApplier.ApplyFrame(frameName);
                break;
            case "Gift":
                TurnOffEveryCanvas();
                giftCanvas.SetActive(true);

                // 필터, 프레임 해금
                filterName = ConvertEpisodeToFilterName(step.EpisodeID);
                frameName = ConvertEpisodeToFrameName(step.EpisodeID);

                FilterFrameManager.instance.Unlockfilter(filterName);
                FilterFrameManager.instance.Unlockframe(frameName);

                // 해금 내용 표시
                string rawText = step.ScriptID;
                string processedText = rawText.Replace("\\n", "\n");
                giftInfoText.text = processedText; // 변경될 수 있음
                StartCoroutine(PopupAnimator.OnPanelPopup(giftAlarmPanel)); // 패널 표시(애니메이션 적용)

                // 다음 스토리 해금을 위한 데이터 저장


                BondScoreDataManager.Instance.SaveFinalBondScore(episodeBondScore); // 유대감 점수 영구 저장
                break;
        }

        if (step.NextID == 0)
        {
            Debug.Log("스토리 종료 지점에 도달했습니다.");
            EndEpisode();
            return;
        }
    }

    public void NextStep()
    {
        nextDialogueBtn.interactable = true;

        int nextID = CurrentEpisode[CurrentID].NextID;
        ShowCurrentID(nextID);
    }
    // EmotionCamera 뒤의 대사와 유대감 점수 시스템을 위한 함수
    public void NextDialogue(int nextID)
    {
        nextDialogueBtn.interactable = true;
        ShowCurrentID(nextID);
    }
    #endregion

    #region Canvas Control
    // 캔버스 전환을 위한 모든 캔버스 비활성화 함수
    // NameInput 시에는 캔버스 비활성화 하지 않음
    void TurnOffEveryCanvas()
    {
        loadingCanvas.SetActive(false);
        dialogueCanvas.SetActive(false);
        choiceCanvas.SetActive(false);
        inputFieldCanvas.SetActive(false);
        cameraCanvas.SetActive(false);
        emotionCameraCanvas.SetActive(false);
        storyCameraCanvas.SetActive(false);
        HideFaceSearching();
        giftCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }
    #endregion



    //#region Player Name
    //// 플레이어 이름 불러오기
    //public void LoadPlayerName()
    //{
    //    if (PlayerPrefs.HasKey("PlayerName"))
    //    {
    //        PlayerName = PlayerPrefs.GetString("PlayerName");
    //    }
    //    else
    //    {
    //        PlayerName = "주인공";
    //    }
    //}
    //#endregion

    #region Image Utility
    // 배경, 캐릭터 Image 변경
    // 사진 존재 x, 사진 설정 x, 사진 설정 o로 경우를 나눠 설정
    // Get: 타겟이미지(배경 이미지 오브젝트/캐릭터 이미지 오브젝트), MainStory 기반 imageID
    void SetImage(UnityEngine.UI.Image targetImg, string imgPath)
    {
        if (string.IsNullOrEmpty(imgPath)) // 사진 존재 x 시
        {
            //print("사진 존재 x");
            return; // 사진 변경x
        }
        if (imgPath == "NONE")
        {
            targetImg.sprite = null; // 사진 제거
            targetImg.color = new Color(1, 1, 1, 0);
            //print("사진 제거");
            return;
        }

        Sprite newSprite = Resources.Load<Sprite>($"Arts/{imgPath}");
        
        if (newSprite != null)
        {
            targetImg.color = new Color(1, 1, 1, 1);
            targetImg.sprite = newSprite;
            //print("사진 변경");
        }
        else
        {
            Debug.LogWarning($"[ImageChanger] 이미지 로드 실패: Arts/{imgPath}, 기본 이미지 사용");

            if (targetImg.name.Contains("background"))
            {
                SetDefaultBackground(targetImg);
            }
        }
    }
    // 기본 배경 설정
    // 배경 이미지 존재x 시 내장된 기본 배경으로 변경
    // 배경이 없을 경우 이상하기 때문에 예외처리용으로 설정
    void SetDefaultBackground(UnityEngine.UI.Image targetImg)
    {
        Sprite defaultSprite = Resources.Load<Sprite>($"Arts/2Back/defaultBackground");
        if (defaultSprite != null)
        {
            targetImg.sprite = defaultSprite;
        }
    }
    #endregion

    #region Show/Hide ARCamera&StoryCamera Btn, FaceSearching Panel(EmotionCamera, StoryCamera)
    void ShowARView()
    {
        arCamera.enabled = true;
        storyCamera.enabled = false;
        arSession.enabled = true;
    }

    void ShowStoryView()
    {
        arCamera.enabled = false;
        storyCamera.enabled = true;
        arSession.enabled = false;
    }
    
    // 카메라 실행 시 카메라 버튼 인터렉션 활성화/비활성화
    public void ActivateCameraBtnInteraction()
    {
        emoCameraBtn.interactable = true;
        arCameraBtn.interactable = true;
    }
    public void DeActivateCameraBtnInteraction()
    {
        emoCameraBtn.interactable = false;
        arCameraBtn.interactable = false;
    }

    // 표정 탐색 중 표정 탐색 패널 보여주기/숨기기
    public void ShowFaceSearching()
    {
        faceSearchingPanel.SetActive(true);
    }
    public void HideFaceSearching()
    {
        faceSearchingPanel.SetActive(false);
    }

    #endregion

    #region Update 유대감 점수와 다이얼로그(EmotionCamera)
    // 유대감 점수 업데이트
    // dominantEmotion에 따라 점수 반경
    // emotionCamera > Dialogue
    public void UpdateDialogueAndBond(Emotion dominantEmotion)
    {
        // 감정에 따라 다음 ID 확정
        int emotionIdx = 1;
        switch (dominantEmotion)
        {
            case Emotion.Happy:
                emotionIdx = 1;
                break;
            case Emotion.Sad:
                emotionIdx = 2;
                break;
            case Emotion.Suprise:
                emotionIdx = 3;
                break;
            case Emotion.Angry:
                emotionIdx = 4;
                break;
            default:
                emotionIdx = 1;
                break;
        }

        int nextID = CurrentID + emotionIdx;

        // 유대감 점수 증가
        episodeBondScore += CurrentEpisode[nextID].Score;
        Debug.Log($"현재 유대감 점수: {episodeBondScore}");

        // 유대감 슬라이드 내역 반영
        bondSlider.value = episodeBondScore;

        // 카메라 버튼 활성화 및 표정 탐색 패널 숨기기
        ActivateCameraBtnInteraction();
        HideFaceSearching();

        // 대사 띄우기(다음 단계)
        NextDialogue(nextID);
        //NextStep();
    }
    // 아니면 id를 1씩 증가시켜서 NextDialogue(currentID + 1 + a)로 바꾼다거나
    // a는 0행복 1슬픔 2놀람 3화남 인거고
    // 그리고 다이얼로그 저거 함수로 바꿔서
    // AddBond()
    // Dialogue()
    // 뭐 이런 식으로 유대감 증가시키고 다이얼로그 표시되게 하는 것도 괜찮은 듯

    public void UpdateAffection(float bondChange)
    {
        BondScore += bondChange;
        // 유대감 점수 출력 (디버그용)
        Debug.Log($"현재 유대감 점수: {BondScore}");
    }

    // BondScore 잘 업데이트 되는지 테스트
    public void CheckBondScoreUpdate()
    {
        UpdateDialogueAndBond(Emotion.Happy);
    }
    #endregion

    #region 필터와 프레임 적용(StoryCamera)
    // 에피소드ID를 필터 이름으로 변환
    private string ConvertEpisodeToFilterName(string episodeID)
    {
        // "Episode1" → "Ep1"
        if (episodeID.StartsWith("Episode"))
        {
            string number = episodeID.Substring("Episode".Length); // "1"
            return $"Ep{number}";
        }

        Debug.LogWarning($"Invalid episodeID: {episodeID}");
        return null;
    }

    // 에피소드ID를 프레임 이름으로 변환
    private string ConvertEpisodeToFrameName(string episodeID)
    {
        // "Episode1" → "Ep1_Frame"
        if (episodeID.StartsWith("Episode"))
        {
            string number = episodeID.Substring("Episode".Length); // "1"
            return $"Ep{number}_Frame";
        }

        Debug.LogWarning($"Invalid episodeID: {episodeID}");
        return null;
    }
    #endregion

    #region Choice DB
    // 누른 Choice의 내용 저장 후 StoryCamera 실행 시 찍은 사진과 DB에 넣을 수 있도록 하기
    // 사진 찍은 이후 사진 데이터 저장
    public void SaveStoryPhoto(string filepath, string capturedAt)
    {
        storyPhoto_filepath = filepath;
        storyPhoto_capturedAt = capturedAt;
    }
    // 선택지 선택 이후 선택지 텍스트 저장 및 DB에 저장
    public void OnClickSaveChoiceText(Button clickedBtn)
    {
        Debug.Log("SaveChoiceText 클릭됨");
        MainStory step = CurrentEpisode[CurrentID-1];
        print(step.EventType);
        if (step.EventType == "SaveChoice")
        {
            // 선택한 버튼의 텍스트 저장
            TextMeshProUGUI tmp = clickedBtn.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log("선택한 선택지의 텍스트: " + tmp.text);
            storyPhoto_mood = tmp.text;

            Debug.Log(storyPhoto_filepath + " " + storyPhoto_capturedAt + " " + storyPhoto_mood);

            // DB에 저장
            SaveStoryPhotoDatas();
        }
    }
    // 사진 데이터들 DB에 저장
    void SaveStoryPhotoDatas()
    {
        // episodeID, episodeTitle 가져오기
        string episodeId = CurrentEpisode[CurrentID].EpisodeID;
        string episodeTitle = CurrentEpisode[CurrentID].ChapterTitle;

        // DB에 저장
        emotionGalleryDBWriter.InsertStoryPhoto(storyPhoto_filepath, storyPhoto_capturedAt, episodeId, episodeTitle, storyPhoto_mood);
    }

    // 재실행 시 사진 데이터 초기화
    private void ResetStoryPhotoData()
    {
        storyPhoto_filepath = "";
        storyPhoto_capturedAt = "";
        storyPhoto_mood = "";
    }
    #endregion

    void EndEpisode()
    {

    }

    
}
