using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FilterManager : MonoBehaviour
{
    public static FilterManager instance;
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

    private Dictionary<string, bool> filterUnlocked = new Dictionary<string, bool>
    {
        // 추후 key는 애셋명으로 변경할 예정
        {"Episode1", false},
        {"Episode2", false },
        {"Episode3", false },
        {"Episode4", false },
        {"HiddenMission1", false },
        {"HiddenMission2", false }
    };

    private Dictionary<string, string> hiddenMissionFilterName = new Dictionary<string, string>
    {
        // 추후 key: 애셋명, value: 필터에 맞는 설명으로 변경할 예정
        {"HiddenMission1", "외계인 느낌의 멋진 안경 👽✨"},
        {"HiddenMission2", "외계인 느낌의 멋진 안경 👽✨" }
    };

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

    public string[] GetFilterUnlockedKeys()
    {
        return filterUnlocked.Keys.ToArray();
    }

    public bool[] GetFilterUnlockedValues()
    {
        return filterUnlocked.Values.ToArray();
    }
}
