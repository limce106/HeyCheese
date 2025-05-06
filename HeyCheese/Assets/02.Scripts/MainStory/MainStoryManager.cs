using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ���丮<->�̴ϰ��� �� �ش� ������ ����, ���Ǿ�� ������
// ���丮 -> ��� ����, ���� �ʱ�ȭ �ʿ�
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
    public Dictionary<int, MainStory> currentEpisode; // ������ ���Ǽҵ忡 �ش��ϴ� {id:MainStory CSV�� �� ��(step), ..}
    private int currentID = 0;
    public static string PlayerName = "���ΰ�";

    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� �Ѿ�� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    //public void Clear()
    //{
    //    selectedLevel = 0;
    //    selectedEmotion = "None";
    //    selectedSituation = "None";
    //}

    //// ���� �� �Ѿ�� �� ȣ�� �ʿ�
    //public void DestroySelf()
    //{
    //    Clear();
    //    Destroy(gameObject);
    //    Instance = null;
    //}

    //// !!���� �� ���� �� �Ʒ� �ۼ����ֱ�!!
    ////void Start()
    ////{
    ////    if (StoryManagerTest.Instance != null)
    ////    {
    ////        StoryManagerTest.Instance.DestroySelf();
    ////    }
    ////}

    void Start()
    {
        // ���Ǽҵ� �޴����� ���õ� 
        string episodeID = PlayerPrefs.GetString("SelectedEpisodeID");
        print("�Ŵ���: " + episodeID);

        // �Ľ��� ���� �ҷ�����
        CSVParser csvParser = new CSVParser();
        currentEpisode = csvParser.ParseMainStories(episodeID);
        Debug.Log($"[MainStoryManager] Loaded EpisodeID: {episodeID}");
        print(currentEpisode.Keys);
        print(currentEpisode.Values);
        currentID = 0;

        // ������ �� ����� �̸� �ҷ�����
        LoadPlayerName();

        //loadingCanvas.gameObject.SetActive(true);
        ShowCurrentID(currentID);
    }

    //// �޴����� ��ư ������ ��
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
    //        Debug.LogError("StoryDataTest.json ������ ã�� �� �����ϴ�!");
    //        return;
    //    }

    //    string jsonString = File.ReadAllText(path);

    //    try
    //    {
    //        var allData = JsonHelperTest.FromJson<StoryDataTest>(jsonString);

    //        if (allData == null)
    //        {
    //            Debug.LogError("JSON �Ľ� ����! allData�� null�Դϴ�.");
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
    //        Debug.LogError($"JSON �Ľ� �� ���� �߻�: {e.Message}");
    //    }
    //}

    // ���� ����(ID)�� �´� �̺�Ʈ ����
    void ShowCurrentID(int id)
    {
        MainStory step = currentEpisode[id];
        currentID = id;

        if (step.NextID == 0)
        {
            Debug.Log("���丮 ���� ������ �����߽��ϴ�.");
            EndEpisode();
            return;
        }

        // �߸��� ���� �ǳʶٱ�
        // id�� -1(����o)�� ���, �ش� ����(csv�� ��)�� ��ŵ��
        if (id == -1)
        {
            Debug.LogWarning($"[MainStory] ID �Ľ� ����: ���� ����(��): {step}");
            NextStep();
        }

        switch (step.EventType)
        {
            case "Loading":
                print("Loading");
                TurnOffEveryCanvas();
                loadingCanvas.SetActive(true);

                
                break;
            case "Video":
                print("Video ���");
                print("Video ���");
                break;
            case "Dialogue":
                TurnOffEveryCanvas();
                dialogueCanvas.SetActive(true);

                break;
            case "Choice":
                TurnOffEveryCanvas();
                choiceCanvas.SetActive(true);

                choiceQuestionText.text = step.ScriptID; // ��� ����
                choice1Text.text = step.Choice1; // ������ ����
                choice2Text.text = step.Choice2; // ������ ����
                choice3Text.text = step.Choice3; // ������ ����

                break;
            case "NameInput":
                // ��� ��ư ����

                inputFieldCanvas.SetActive(true);
                //playerNameManager.SavePlayerName();

                //PlayerName = PlayerNameManager.PlayerName; // ���Ǽҵ�1�� ��� �� �ܰ� ���� �÷��̾��� �̸� ������Ʈ


                // OnConfirmYes ��, �÷��̾� �̸� ������Ʈ �� ���� �ܰ�� �̵�



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
            //    dialogueText.text = "��� ȭ���Դϴ�.";
            //    break;
        }
    }

    public void NextStep()
    {
        int nextID = currentEpisode[currentID].NextID;
        ShowCurrentID(nextID);
    }

    // ĵ���� ��ȯ�� ���� ��� ĵ���� ��Ȱ��ȭ �Լ�
    // NameInput �ÿ��� ĵ���� ��Ȱ��ȭ ���� ����
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

    // �÷��̾� �̸� �ҷ�����
    void LoadPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }
        else
        {
            PlayerName = "���ΰ�";
        }
    }

    void EndEpisode()
    {

    }
}
