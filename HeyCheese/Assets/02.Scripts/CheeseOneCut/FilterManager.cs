using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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
        // 추후 key: 애셋명, value: 필터명으로 변경할 예정
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

    public void Unlock(string key)
    {
        if(filterUnlocked.ContainsKey(key))
        {
            filterUnlocked[key] = true;
            StartCoroutine(OnHiddenMissionPopup(hiddenMissionFilterName[key]));
        }
    }

    private IEnumerator OnHiddenMissionPopup(string filterName)
    {
        GameObject HiddenMissionPanel = GameObject.Find("Canvas/Panel_HiddenMission");
        if(HiddenMissionPanel == null)
        {
            Debug.LogError("패널을 찾을 수 없습니다.");
            yield break;
        }

        TextMeshPro missionClearText = HiddenMissionPanel.GetComponent<TextMeshPro>();
        if (missionClearText == null)
        {
            Debug.LogError("텍스트를 찾을 수 없습니다.");
            yield break;
        }

        missionClearText.text = "🎁 선물 🎁 \n AR 패션 아이템: " + filterName;
        HiddenMissionPanel.SetActive(true);

        yield return new WaitForSeconds(3f);

        HiddenMissionPanel.SetActive(false);
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
