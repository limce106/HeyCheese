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
    //[SerializeField] private AudioSource[] UISoundLoopPlayer; // 효과음 플레이어 (반복)

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // 배경음들
    [SerializeField] private AudioClip[] soundEffectClip; // 효과음들
                                                          //[SerializeField] private AudioClip[] UISoundClip_LOOP; // UI 효과음들

    [Header("Sound Slider")]
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SoundEffectSlider;


    private float bgmVolume = 0.5f;
    private Coroutine bgmCoroutine;

    // 동시에 여러 효과음들이 플레이 될 수도 있으므로 여러 플레이어를 두고 순차적으로 실행하기 위한 변수
    private int soundEffectPlayerCursor;

    // BGM 전용 열거형 변수 추가하고 sound Player의 인스펙터 창의 bgmClip도 해당 int 변수 값 index대로 추가하여 사용
    // ex) MINIGAME4_SLEEPING을 index 2로 추가하여 해당 위치에 audioClip 추가
    public enum BGM{
        STOP = -1,
        OPENING = 0,
        DEFAULT = 1,
        MINIGAME4_SLEEPING = 2,
        MINIGAME4_DANCING = 3,
    }

    public enum SFX
    {
        STOP = -1,
        KNOCKING = 0,
        OPENDOOR = 1,
        CLOSEDOOR = 2,
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
        bgmCoroutine = StartCoroutine(ChangeBGMFade((int)BGM.OPENING));
    }

    public void ChangeVolume(float bgmVolume, float soundEffectVolume)
    {
        if (bgmVolume != -1)
        {
            this.bgmVolume = bgmVolume;
            bgmPlayer.volume = bgmVolume;
        }

        if (soundEffectVolume != -1)
        {
            foreach (AudioSource audio in soundEffectPlayer) audio.volume = soundEffectVolume;
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
        if (bgm != (int)BGM.STOP)
        {
            bgmPlayer.clip = bgmClip[bgm];
            bgmPlayer.Play();
        }

        // 볼륨을 다시 키우기
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            if (bgm != (int)BGM.STOP) bgmPlayer.volume = Mathf.Lerp(0, bgmVolume, currentTime / fadeDuration);
            else bgmPlayer.volume = Mathf.Lerp(bgmVolume, 0, currentTime / fadeDuration);

            yield return null;
        }

        // 최종 볼륨 설정
        if (bgm != (int)BGM.STOP) bgmPlayer.volume = bgmVolume;
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

    // 소리 설정 패널에서 슬라이더에 넣음
    public void ChangeSoundValue(string playerName)
    {
        //TMP_Text text;
        Slider slider;

        if (playerName == "BGMSlider")
        {
            //text = uiGameObjects[eUIGameObjectName.BGMValue].GetComponent<TextMeshProUGUI>();
            slider = BGMSlider;
            ChangeVolume(slider.value, -1);
        }
        else
        {
            //text = uiGameObjects[eUIGameObjectName.SoundEffectValue].GetComponent<TextMeshProUGUI>();
            slider = SoundEffectSlider;
            ChangeVolume(-1, slider.value);
        }

        //text.text = (slider.value * 100).ToString("F0");
    }
}
