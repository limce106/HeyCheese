using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WelcomeMenuController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private float videoDuration = 0f;
    private bool videoStarted = false;

    public Slider loadingSlider;

    private float timer = 0f;

    void Start()
    {
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer videoPlayer)
    {
        videoDuration = (float)videoPlayer.length;
        videoPlayer.Play();
        videoStarted = true;
    }

    void Update()
    {
        UpdateLoadingBar();
    }

    void UpdateLoadingBar()
    {
        if (!videoStarted || videoDuration <= 0f)
            return;

        timer += Time.deltaTime;
        loadingSlider.value = timer / videoDuration;

        if(timer >= videoDuration)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
