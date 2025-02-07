using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider enemySlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambientSlider;

    private void Awake()
    {
        LoadVolume();
    }

    public void SetMainVolume()
    {
        float volum = mainSlider.value;
        mixer.SetFloat("Main", Mathf.Log10(volum)*20);
        PlayerPrefs.SetFloat("MainVolume",volum);
    }

    public void SetEnemyVolume()
    {
        float volum = enemySlider.value;
        mixer.SetFloat("Enemy", Mathf.Log10(volum) * 20);
        PlayerPrefs.SetFloat("EnemyVolume", volum);
    }
    public void SetSFXVolume()
    {
        float volum = sfxSlider.value;
        mixer.SetFloat("SFX", Mathf.Log10(volum) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volum);
    }
    public void SetMusicVolume()
    {
        float volum = musicSlider.value;
        mixer.SetFloat("Music", Mathf.Log10(volum) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volum);
    }
    public void SetAmbientVolume()
    {
        float volum = ambientSlider.value;
        mixer.SetFloat("Ambient", Mathf.Log10(volum) * 20);
        PlayerPrefs.SetFloat("AmbientVolume", volum);
    }

    private void LoadVolume()
    {
        mainSlider.value = PlayerPrefs.GetFloat("MainVolume");
        enemySlider.value = PlayerPrefs.GetFloat("EnemyVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        ambientSlider.value = PlayerPrefs.GetFloat("AmbientVolume");
        SetVolume();
    }

    public void SetVolume()
    {
        SetAmbientVolume();
        SetEnemyVolume();
        SetMainVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
}
