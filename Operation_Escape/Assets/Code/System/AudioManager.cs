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
    public AudioSource newMusicBGMSource;
    public AudioSource sfxSource;

    [Header("----- Audio Clip -----")]
    public AudioClip backGround;
    //public AudioClip[] backGroundBoss;
    public AudioClip gameOver;
    public AudioClip click;

    private void Awake()
    {
        audioManager = this;
    }

    private void Start()
    {
        musicBGMSource.clip = backGround;
        musicBGMSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public async UniTask ChangeBGM(AudioClip newClip)
    {
        if (musicBGMSource.clip == newClip) return;

        newMusicBGMSource.clip = newClip;
        newMusicBGMSource.volume = 0;
        newMusicBGMSource.Play();

        await CrossfadeBGM();

        musicBGMSource.Stop();
        (musicBGMSource, newMusicBGMSource) = (newMusicBGMSource, musicBGMSource);
    }

    private async UniTask CrossfadeBGM()
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
