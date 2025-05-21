using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 이름 설정 및 저장
// 게임 초반부에 있어야 함
// Non-lazy, DDOL
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public string PlayerName { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지

        LoadPlayerName();
    }

    // 플레이어 이름 설정
    public void SetPlayerName(string name)
    {
        PlayerName = name;
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
        Debug.Log($"플레이어 이름 저장됨: {PlayerName}");
    }

    // 플레이어 이름 불러오기
    public void LoadPlayerName()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", "주인공");
    }
}
