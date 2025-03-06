using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;
    public float fadeDuration = 2f;
    [Header("----- Audio Soure -----")]
    public AudioSource musicBGMSource;
    public AudioSource musicAMSource;
    public AudioSource newMusicBGMSource;
    public AudioSource sfxSource;

    [Header("----- Audio Clip -----")]
    public AudioClip backGround;
   
    public AudioClip ambient;
    //public AudioClip[] backGroundBoss;
    public AudioClip gameOver;
    public AudioClip click;
    [HideInInspector]
    public AudioClip oldBackGround;

    private void Awake()
    {
        audioManager = this;
    }

    private void Start()
    {
        musicBGMSource.clip = backGround;
        musicBGMSource.Play();
        musicAMSource.clip = ambient;
        musicAMSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public async UniTask ChangeBGM(AudioClip newClip , float fade)
    {
        float oldfade = fadeDuration;
        fadeDuration = fade;
        await ChangeBGM(newClip);
        fadeDuration = oldfade;
    }

    public async UniTask ChangeBGM(AudioClip newClip)
    {
        if (musicBGMSource.clip == newClip) return;
        oldBackGround = backGround;
        newMusicBGMSource.clip = newClip;
        newMusicBGMSource.volume = 0;
        newMusicBGMSource.Play();

        await CrossfadeBGM();

        musicBGMSource.Stop();
        (musicBGMSource, newMusicBGMSource) = (newMusicBGMSource, musicBGMSource);
    }

    public async UniTask CrossfadeBGM()
    {
        float timer = 0f;
        float startVolume = musicBGMSource.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            musicBGMSource.volume = Mathf.Lerp(startVolume, 0, progress);
            newMusicBGMSource.volume = Mathf.Lerp(0, startVolume, progress);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
