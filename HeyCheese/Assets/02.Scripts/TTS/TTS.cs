using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTS : MonoBehaviour
{
    public Text readKoreaText;

    private string korea = "Ko";
    private string url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=";

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ReadText();
    }

    IEnumerator PlaySpeak(string str)
    {
        WWW www = new WWW(str);
        yield return www;

        audioSource.clip = www.GetAudioClip(false, true, AudioType.MPEG);
        audioSource.Play();
    }

    private string getString(string text, string stateName)
    {
        return text + "&tl=" + stateName + "-gb";
    }

    public void ReadText()
    {
        StartCoroutine(PlaySpeak(url + getString(readKoreaText.text, korea)));
    }
}
