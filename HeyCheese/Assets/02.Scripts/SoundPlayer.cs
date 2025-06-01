using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmPlayer; // 배경음 플레이어
    [SerializeField] private AudioSource[] soundEffectPlayer; // 효과음 플레이어들
    [SerializeField] private AudioSource ttsPlayer;
    [SerializeField] private AudioSource touchSFXPlayer;    // 터치 효과음 플레이어 
    // 터치효과음은 기다리는 순서 없이 바로 나와야 하기 때문에 soundEffectPlayer말고 따로 플레이어 사용.
    //[SerializeField] private AudioSource[] UISoundLoopPlayer; // 효과음 플레이어 (반복)

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // 배경음들
    [SerializeField] private AudioClip[] soundEffectClip; // 효과음들
    [SerializeField] private AudioClip touchSFXClip;    // 터치 효과음

    [Header("Sound Slider")]
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SoundEffectSlider;
    [SerializeField] private Slider TtsSlider;

    // TTS
    private string korea = "Ko";
    private string url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=";

    private float bgmVolume = 0.5f;
    private Coroutine bgmCoroutine;

    // 동시에 여러 효과음들이 플레이 될 수도 있으므로 여러 플레이어를 두고 순차적으로 실행하기 위한 변수
    private int soundEffectPlayerCursor;

    // BGM 전용 열거형 변수 추가하고 sound Player의 인스펙터 창의 bgmClip도 해당 int 변수 값 index대로 추가하여 사용
    // ex) MINIGAME4_SLEEPING을 index 2로 추가하여 해당 위치에 audioClip 추가
    public enum BGM{
        STOP_BGM = -1,
        OPENING_BGM = 0,
        DEFAULT_BGM = 1,
        MINIGAME4_BGM_SLEEPING = 2,
        MINIGAME4_BGM_DANCING = 3,
        Prolog_BGM = 4,
        StoryEpi1_BGM = 5,
        StoryEpi2_BGM = 6,
        StoryEpi3_BGM = 7,
        StoryEpi4_BGM = 8,
    }

    public enum SFX
    {
        STOP_SFX = -1,
        KNOCKING_SFX = 0,
        OPENDOOR_SFX = 1,
        CLOSEDOOR_SFX = 2,
        Prolog_SFX_1 = 3,
        StoryEpi1_SFX_1 = 4,
        StoryEpi2_SFX_1 = 5,
        StoryEpi3_SFX_1 = 6,
        StoryEpi4_SFX_1 = 7,
        StoryEpi4_SFX_2 = 8,
        StoryEpi4_SFX_3 = 9,
        StoryEpi_SFX_End = 10,
        Shouting_SFX = 11,
        Shining_SFX =12,
    }

    // 외부에서 string 타입으로 받은 sound ID를 enum의 BGM이나 SFX과 매핑해서 
    // BGM이면 브금 재생, SFX이면 sfx 재생
    public void PlaySoundByID(string soundID)
    {
        if (soundID.Contains("_SFX"))
        {
            if (System.Enum.TryParse<SFX>(soundID, out var sfxValue))
            {
                SoundEffectPlay((int)sfxValue);
            }
            else
            {
                Debug.LogWarning("Invalid SFX ID: " + soundID);
            }
        }
        else if (soundID.Contains("_BGM"))
        {
            if (System.Enum.TryParse<BGM>(soundID, out var bgmValue))
            {
                ChangeBGM((int)bgmValue);
            }
            else
            {
                Debug.LogWarning("Invalid BGM ID: " + soundID);
            }
        }
        else
        {
            Debug.LogWarning("Unknown sound type: " + soundID);
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        bgmCoroutine = StartCoroutine(ChangeBGMFade((int)BGM.OPENING_BGM));
    }

    void Update()
    {
        // 모바일 터치
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlayTouchSound();
        }

        // 에디터/PC 마우스 클릭 (디버그용)
        if (Input.GetMouseButtonDown(0))
        {
            PlayTouchSound();
        }
    }

    void PlayTouchSound()
    {
        if (touchSFXClip != null)
        {
            touchSFXPlayer.PlayOneShot(touchSFXClip);
        }
    }

    public void ChangeVolume(float bgmVolume, float soundEffectVolume, float ttsVolume)
    {
        if (bgmVolume != -1)
        {
            this.bgmVolume = bgmVolume;
            bgmPlayer.volume = bgmVolume;
        }

        if (soundEffectVolume != -1)
        {
            foreach (AudioSource audio in soundEffectPlayer) audio.volume = soundEffectVolume;
            touchSFXPlayer.volume = soundEffectVolume;
        }

        if (ttsVolume != -1)
        {
            ttsPlayer.volume = ttsVolume;
        }
    }

    // 이 메소드로 사용해서 챕터나 미니게임의 Awake에서 바꾸면 됨.
    public void ChangeBGM(int bgm)
    {
        StopCoroutine(bgmCoroutine);
        bgmCoroutine = StartCoroutine(ChangeBGMFade(bgm));
    }

    private IEnumerator ChangeBGMFade(int bgm)
    {
        float fadeDuration = 2f;
        float currentTime = 0f;

        // 새로운 BGM 설정 및 재생
        if (bgm != (int)BGM.STOP_BGM)
        {
            bgmPlayer.clip = bgmClip[bgm];
            bgmPlayer.Play();
        }

        // 볼륨을 다시 키우기
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            if (bgm != (int)BGM.STOP_BGM) bgmPlayer.volume = Mathf.Lerp(0, bgmVolume, currentTime / fadeDuration);
            else bgmPlayer.volume = Mathf.Lerp(bgmVolume, 0, currentTime / fadeDuration);

            yield return null;
        }

        // 최종 볼륨 설정
        if (bgm != (int)BGM.STOP_BGM) bgmPlayer.volume = bgmVolume;
    }


    public void SoundEffectPlay(int num)
    {
        // 재생할 효과음 변경
        soundEffectPlayer[soundEffectPlayerCursor].clip = soundEffectClip[num];

        // 음악 재생
        soundEffectPlayer[soundEffectPlayerCursor].Play();

        // 다음 효과음 Player로 넘긴다
        soundEffectPlayerCursor = (soundEffectPlayerCursor + 1) % soundEffectPlayer.Length;
    }

    public bool isSoundEffectPlaying()
    {
        return soundEffectPlayer[soundEffectPlayerCursor].isPlaying;
    }


    // -1을 넣으면 모든 효과음 종료 (배경음 제외)
    public void SoundEffectStop(int num)
    {
        foreach (AudioSource audio in soundEffectPlayer)
        {
            if (num == -1) { audio.Stop(); continue; }
            if (audio.clip == soundEffectClip[num]) audio.Stop();
        }
    }

    // TTS 재생
    IEnumerator PlaySpeak(string str)
    {
        WWW www = new WWW(str);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("TTS 다운로드 오류: " + www.error);
            yield break;
        }

        if (www.bytesDownloaded == 0)
        {
            Debug.LogError("TTS 응답이 비어 있음!");
            yield break;
        }

        AudioClip clip = www.GetAudioClip(false, true, AudioType.MPEG);
        if (clip == null)
        {
            Debug.LogError("AudioClip 생성 실패: clip == null");
            yield break;
        }

        ttsPlayer.clip = clip;
        ttsPlayer.Play();
    }

    private string getString(string text, string stateName)
    {
        return text + "&tl=" + stateName + "-gb";
    }

    public void ReadText(string scriptText)
    {
        StartCoroutine(PlaySpeak(url + getString(scriptText, korea)));
    }


    // 소리 설정 패널에서 슬라이더에 넣음
    public void ChangeSoundValue(string playerName)
    {
        //TMP_Text text;
        Slider slider;

        switch (playerName)
        {
            case "BGMSlider":
                slider = BGMSlider;
                ChangeVolume(slider.value, -1, -1);
                break;
            case "SoundEffectSlider":
                slider = SoundEffectSlider;
                ChangeVolume(-1, slider.value, -1);
                break;
            case "TtsSlider":
                slider = TtsSlider;
                ChangeVolume(-1, - 1, slider.value);
                break;

        }
    }
}
