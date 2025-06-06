using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public static class PlayerPrefsControll
{
    [System.Serializable]
    public class PrefEntry
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class PrefEntryList
    {
        public List<PrefEntry> entries = new List<PrefEntry>();
    }

    [System.Serializable]
    public class PrefDictionary
    {
        public List<string> keys = new();
        public List<bool> values = new();

        public PrefDictionary() { }

        public PrefDictionary(Dictionary<string, bool> dict)
        {
            foreach(var kv in dict)
            {
                keys.Add(kv.Key);
                values.Add(kv.Value);
            }
        }

        public Dictionary<string, bool> ToDictionary()
        {
            Dictionary<string, bool> result = new();
            for(int i = 0; i < keys.Count; i++)
            {
                result[keys[i]] = values[i];
            }

            return result;
        }
    }

    #region PlayerPrefs 저장
    public static void SavePref_SetString(string key, string value)
    {
        // 실제 데이터 저장
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();

        // 목록에 저장
        SavePref(key, value);
    }
    public static void SavePref_SetInt(string key, int value)
    {
        // 실제 데이터 저장
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();

        // 목록에 저장
        SavePref(key, value.ToString());
    }

    public static void SavePref(string key, string value)
    {
        // 기존 목록 가져오기
        string json = PlayerPrefs.GetString("AllKeyValuePrefs", "{}");
        PrefEntryList list = JsonUtility.FromJson<PrefEntryList>(json) ?? new PrefEntryList();

        // 이미 존재하면 덮어쓰기
        var existing = list.entries.Find(e => e.key == key);
        if (existing != null)
        {
            existing.value = value;
        }
        else
        {
            list.entries.Add(new PrefEntry { key = key, value = value });
        }

        // 다시 저장
        string updatedJson = JsonUtility.ToJson(list);
        PlayerPrefs.SetString("AllKeyValuePrefs", updatedJson);
        PlayerPrefs.Save();
    }
    #endregion

    #region PlayerPrefs 출력
    public static void PrintAllPrefs()
    {
        string json = PlayerPrefs.GetString("AllKeyValuePrefs", "{}");
        PrefEntryList list = JsonUtility.FromJson<PrefEntryList>(json);

        if (list == null || list.entries.Count == 0)
        {
            Debug.Log("저장된 키 없음");
            return;
        }

        foreach (var entry in list.entries)
        {
            Debug.Log($"[Pref] {entry.key}: {entry.value}");
        }
    }
    #endregion


    #region PlayerPrefs 삭제
    // 에피소드 해금 관련 PlayerPrefs 초기화
    public static void DeleteEpisodeUnLock()
    {
        PlayerPrefs.DeleteKey("Prolog_Cleared");
        PlayerPrefs.DeleteKey("Episode1_Cleared");
        PlayerPrefs.DeleteKey("Episode2_Cleared");
        PlayerPrefs.DeleteKey("Episode3_Cleared");
        PlayerPrefs.DeleteKey("Episode4_Cleared");
    }
    #endregion
}
