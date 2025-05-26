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
        yield return new WaitForSeconds(2f);

        MainStoryGameManager.MainStoryGM.NextStep();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainStory");
    }
}
