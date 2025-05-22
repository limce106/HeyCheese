using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MiniGameManager : MonoBehaviour
{
    [Header("패널들")]
    [SerializeField] protected GameObject GuidePanel;
    [SerializeField] protected GameObject ClearPanel;

    protected void Awake()
    {
        GuidePanel.SetActive(true);
        ClearPanel.SetActive(false);
    }

    public abstract void StartGame();

    public void BackToMainStory()
    {
        MainStoryGameManager.MainStoryGM.NextStep();
    }
}
