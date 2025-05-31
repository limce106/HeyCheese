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

    public IEnumerator BackToMainStory()
    {
        Debug.Log("2초 후 메인 스토리로 이동.");
        yield return new WaitForSecondsRealtime(2f); // ← 시간 정지 무시하고 2초 기다림

        MainStoryGameManager.MainStoryGM.NextStep();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainStory");
    }
}
