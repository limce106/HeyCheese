using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using UnityEngine.InputSystem;
using static PlayerPrefsControll;

public class FilterFrameManager : MonoBehaviour
{
    private Dictionary<string, bool> frameUnlocked = new Dictionary<string, bool>
    {
        {"Ep1_Frame", false},
        {"Ep2_Frame", false },
        {"Ep3_Frame", false },
        {"Ep4_Frame", false },
        {"CheeseTheme_Frame", true },
        {"BugiTheme_Frame", true }
    };

    private Dictionary<string, bool> filterUnlocked = new Dictionary<string, bool>
    {
        {"Ep1", false},
        {"Ep2", false },
        {"Ep3", false },
        {"Ep4", false },
        {"Mission1", false },
        {"Mission2", false }
    };

    private Dictionary<string, string> hiddenMissionFilterMessage = new Dictionary<string, string>
    {
        {"Mission1", "외계 고양이 치즈 필터 👽😺"},
        {"Mission2", "부끄럼쟁이 부기 필터 🐢💛" }
    };

    private Coroutine hiddenMissionCoroutine;

    public static FilterFrameManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadFilterFrameUnlockData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ReadOnlyDictionary<string, bool> GetFrameUnlockedReadOnly()
    {
        return new ReadOnlyDictionary<string, bool>(frameUnlocked);
    }

    public ReadOnlyDictionary<string, bool> GetFilterUnlockedReadOnly()
    {
        return new ReadOnlyDictionary<string, bool>(filterUnlocked);
    }

    public void CheckHiddenMission(int faceCount)
    {
        if(faceCount == 1)
        {
            ARFaceFilterApplier arFaceFilterApplier = GameObject.Find("GameManager").GetComponent<ARFaceFilterApplier>();

            if (!filterUnlocked["Mission1"])
            {
                Unlockfilter("Mission1");
            }
            else if(arFaceFilterApplier.IsFilterValid() && !filterUnlocked["Mission2"])
            {
                Unlockfilter("Mission2");
            }
        }
    }

    // 필터 해금
    public void Unlockfilter(string key)
    {
        if(filterUnlocked.ContainsKey(key))
        {
            filterUnlocked[key] = true;

            if(SceneManager.GetActiveScene().name == "CheeseOneCut")
            {
                CheeseOneCutUIManager cheeseOneCutUIManager = GameObject.Find("GameManager").GetComponent<CheeseOneCutUIManager>();
                cheeseOneCutUIManager.EnableFilterButtonByName(key);

                if (hiddenMissionCoroutine != null)
                {
                    StopCoroutine(hiddenMissionCoroutine);
                    hiddenMissionCoroutine = null;
                }

                hiddenMissionCoroutine = StartCoroutine(OnHiddenMissionPopup(hiddenMissionFilterMessage[key]));
            }
        }

        // 현재까지의 필터 해금 여부 저장
        PlayerPrefsControll.SavePref_SetString("FilterUnlocked", JsonUtility.ToJson(new PrefDictionary(filterUnlocked)));
    }

    public void Unlockframe(string key)
    {
        if (frameUnlocked.ContainsKey(key))
        {
            frameUnlocked[key] = true;

            if (SceneManager.GetActiveScene().name == "CheeseOneCut")
            {
                CheeseOneCutUIManager cheeseOneCutUIManager = GameObject.Find("GameManager").GetComponent<CheeseOneCutUIManager>();
                cheeseOneCutUIManager.EnableFrameButtonByName(key);
            }
        }

        // 현재까지의 프레임 해금 여부 저장
        PlayerPrefsControll.SavePref_SetString("FrameUnlocked", JsonUtility.ToJson(new PrefDictionary(frameUnlocked)));
    }

    // 히든 미션 달성 팝업 띄우기
    private IEnumerator OnHiddenMissionPopup(string filterMessage)
    {
        CheeseOneCutUIManager cheeseOneCutUIManager = GameObject.Find("GameManager").GetComponent<CheeseOneCutUIManager>();
        GameObject hiddenMissionPanel = cheeseOneCutUIManager.hiddenMissionPanel;
        if (hiddenMissionPanel == null)
        {
            Debug.LogError("패널을 찾을 수 없습니다.");
            yield break;
        }

        hiddenMissionPanel.SetActive(true);
        TextMeshProUGUI missionClearText = hiddenMissionPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (missionClearText == null)
        {
            Debug.LogError("텍스트를 찾을 수 없습니다.");
            yield break;
        }

        missionClearText.text = "AR 패션 아이템: " + filterMessage;

        yield return PopupAnimator.OnPanelPopup(hiddenMissionPanel);

        hiddenMissionCoroutine = null;
    }

    public void LoadFilterFrameUnlockData()
    {
        string filterJson = PlayerPrefs.GetString("FilterUnlocked", "");
        if(!string.IsNullOrEmpty(filterJson))
        {
            PrefDictionary data = JsonUtility.FromJson<PrefDictionary>(filterJson);

            foreach(var kv in data.ToDictionary())
            {
                if(filterUnlocked.ContainsKey(kv.Key))
                {
                    filterUnlocked[kv.Key] = kv.Value;
                }
            }
        }

        string frameJson = PlayerPrefs.GetString("FrameUnlocked", "");
        if (!string.IsNullOrEmpty(frameJson))
        {
            PrefDictionary data = JsonUtility.FromJson<PrefDictionary>(frameJson);
            
            foreach (var kv in data.ToDictionary())
            {
                if (frameUnlocked.ContainsKey(kv.Key))
                {
                    frameUnlocked[kv.Key] = kv.Value;
                }
            }
        }
    }
}
