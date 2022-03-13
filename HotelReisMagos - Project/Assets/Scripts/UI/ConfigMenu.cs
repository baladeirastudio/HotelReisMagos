using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigMenu : MonoBehaviour
{
    [SerializeField] private Slider mainVolume, musicVolume, narratorVolume;

    [SerializeField] private AudioSource musicSource, narratorSource;

    private void Awake()
    {
        float mainVol, musicVol, narratorVol;

        if (PlayerPrefs.HasKey("mainVol"))
        {
            mainVol = PlayerPrefs.GetFloat("mainVol");
            AudioListener.volume = mainVol;
            mainVolume.SetValueWithoutNotify(mainVol);
        }
        if (PlayerPrefs.HasKey("musicVol"))
        {
            musicVol = PlayerPrefs.GetFloat("musicVol");
            musicSource.volume = musicVol;
            musicVolume.SetValueWithoutNotify(musicVol);
        }
        if (PlayerPrefs.HasKey("narratorVol"))
        {
            narratorVol = PlayerPrefs.GetFloat("narratorVol");
            narratorSource.volume = narratorVol;
            narratorVolume.SetValueWithoutNotify(narratorVol);
        }
    }

    public void SetMainVolume(float newVal)
    {
        AudioListener.volume = newVal;
        PlayerPrefs.SetFloat("mainVol", newVal);
    }
    
    public void SetMusicVolume(float newVal)
    {
        musicSource.volume = newVal;
        PlayerPrefs.SetFloat("musicVol", newVal);
    }
    
    public void SetNarratorVolume(float newVal)
    {
        narratorSource.volume = newVal;
        PlayerPrefs.SetFloat("narratorVol", newVal);
    }
}
