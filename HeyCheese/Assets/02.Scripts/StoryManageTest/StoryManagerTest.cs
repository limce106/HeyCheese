using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class StoryManagerTest : MonoBehaviour
{
    public static StoryManagerTest Instance;

    public Canvas menuCanvas;
    public Canvas dialogueCanvas;
    public Canvas cameraCanvas;

    public Text dialogueText;
    public Button nextButton;

   
    private List<StoryDataTest> currentStory = new List<StoryDataTest>();
    private int currentIndex = 0;

    // 학습 메뉴에서 선택된 내용 저장
    private string selectedEmotion = "None";
    private string selectedSituation = "None";
    private int selectedLevel = 1;


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
    public void Clear()
    {
        selectedLevel = 0;
        selectedEmotion = "None";
        selectedSituation = "None";
    }

    // 메인 씬 넘어갔을 때 호출 필요
    public void DestroySelf()
    {
        Clear();
        Destroy(gameObject);
        Instance = null;
    }

    // !!메인 씬 갔을 때 아래 작성해주기!!
    //void Start()
    //{
    //    if (StoryManagerTest.Instance != null)
    //    {
    //        StoryManagerTest.Instance.DestroySelf();
    //    }
    //}

    void Start()
    {
        // 시작할 땐 메뉴만 보이게
        menuCanvas.gameObject.SetActive(true);
        dialogueCanvas.gameObject.SetActive(false);
        cameraCanvas.gameObject.SetActive(false);

        nextButton.onClick.AddListener(NextStep);
    }

    // 메뉴에서 버튼 눌렀을 시
    public void OnStartStoryButtonClicked(string emotion, string situation, int level)
    {
        selectedEmotion = emotion;
        selectedSituation = situation;
        selectedLevel = level;

        LoadStoryData();
        currentIndex = 0;
        menuCanvas.gameObject.SetActive(false);
        dialogueCanvas.gameObject.SetActive(true);
        ShowCurrentStep();
    }

    void LoadStoryData()
    {
        currentStory.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, "StoryDataTest.json");
        if (!File.Exists(path))
        {
            Debug.LogError("StoryDataTest.json 파일을 찾을 수 없습니다!");
            return;
        }

        string jsonString = File.ReadAllText(path);

        try
        {
            var allData = JsonHelperTest.FromJson<StoryDataTest>(jsonString);

            if (allData == null)
            {
                Debug.LogError("JSON 파싱 실패! allData가 null입니다.");
                return;
            }

            foreach (var item in allData)
            {
                if (item.EmotionID == selectedEmotion &&
                    item.SituationID.StartsWith(selectedSituation) &&
                    (item.Level == selectedLevel.ToString() || item.Level == "all"))
                {
                    currentStory.Add(item);
                }
            }

            currentStory.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 파싱 중 예외 발생: {e.Message}");
        }
    }

    void ShowCurrentStep()
    {
        if (currentIndex >= currentStory.Count)
        {
            Debug.Log("End of story");
            return;
        }

        StoryDataTest step = currentStory[currentIndex];

        switch (step.Type)
        {
            case "Dialogue":
                dialogueText.text = step.Script;
                dialogueCanvas.gameObject.SetActive(true);
                cameraCanvas.gameObject.SetActive(false);
                break;
            case "CameraOnly":
            case "CameraARGuide":
                dialogueCanvas.gameObject.SetActive(false);
                cameraCanvas.gameObject.SetActive(true);
                break;
            case "Result":
                dialogueText.text = "결과 화면입니다.";
                break;
        }
    }

    void NextStep()
    {
        currentIndex++;
        ShowCurrentStep();
    }

    class StoryData
    {
        public string Type;
        public string Script;
        public string NextID;
    }
}
