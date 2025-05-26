using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondScoreDataManager : MonoBehaviour
{
    public static BondScoreDataManager Instance { get; private set; }

    public float BondScore { get; set; } = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지
    }

    // 점수 초기화
    public void ResetBondScore()
    {
        BondScore = 0f;
    }
}
