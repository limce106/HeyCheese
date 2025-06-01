using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
        // 메인 메뉴로 이동 시, 메인 메뉴 브금 재생
        if (sceneName == "MainMenu")
        {
            SoundPlayer.Instance.ChangeBGM((int)SoundPlayer.BGM.DEFAULT_BGM);
        }
    }
}
