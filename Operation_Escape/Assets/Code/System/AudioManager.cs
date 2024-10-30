using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;
    [Header("----- Audio Soure -----")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("----- Audio Clip -----")]
    public AudioClip backGround;
    public AudioClip gameOver;
    public AudioClip click;

    private void Awake()
    {
        audioManager = this;
    }

    private void Start()
    {
        musicSource.clip = backGround;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
