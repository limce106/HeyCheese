using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.ObjectModel;

public class FilterFrameManager : MonoBehaviour
{
    public static FilterFrameManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Dictionary<string, bool> frameUnlocked = new Dictionary<string, bool>
    {
        // 추후 key는 애셋명으로 변경할 예정
        //{"Ep1_Frame", false},
        //{"Ep2_Frame", false },
        //{"Ep3_Frame", false },
        //{"Ep4_Frame", false },
        //{"BuiltIn1", true },
        //{"BuiltIn2", true }

        {"Ep1_Frame", true},
        {"Ep2_Frame", true },
        {"Ep3_Frame", true },
        {"Ep4_Frame", true },
        {"CheeseTheme_Frame", true },
        {"BugiTheme_Frame", true }
    };

    private Dictionary<string, bool> filterUnlocked = new Dictionary<string, bool>
    {
        // 추후 key는 애셋명으로 변경할 예정
        //{"Ep1", false},
        //{"Ep2", false },
        //{"Ep3", false },
        //{"Ep4", false },
        //{"Mission1", false },
        //{"Mission2", false }
        {"Ep1", true},
        {"Ep2", true },
        {"Ep3", true },
        {"Ep4", true },
        {"Mission1", true },
        {"Mission2", true }
    };

    private Dictionary<string, string> hiddenMissionFilterName = new Dictionary<string, string>
    {
        // 추후 key: 애셋명, value: 필터에 맞는 설명으로 변경할 예정
        {"Mission1", "외계 고양이 치즈 필터 👽😺"},
        {"Mission2", "부끄럼쟁이 부기 필터 🐢💛" }
    };

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
        if(faceCount == 2)
        {
            Unlock("HiddenMission1");
        }
        else if (faceCount == 3)
        {
            Unlock("HiddenMission2");
        }
    }

    // 필터 해금
    public void Unlock(string key)
    {
        if(filterUnlocked.ContainsKey(key))
        {
            filterUnlocked[key] = true;

            if(SceneManager.GetActiveScene().name == "CheeseOneCut")
            {
                StartCoroutine(OnHiddenMissionPopup(hiddenMissionFilterName[key]));
            }
        }
    }

    // 히든 미션 달성 팝업 띄우기
    private IEnumerator OnHiddenMissionPopup(string filterName)
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

        missionClearText.text = "🎁 선물 🎁 \n AR 패션 아이템: " + filterName;
        hiddenMissionPanel.SetActive(true);

        yield return new WaitForSeconds(3f);

        hiddenMissionPanel.SetActive(false);
    }
}
