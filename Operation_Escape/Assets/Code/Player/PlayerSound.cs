using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public static PlayerSound playerSound;

    [Header("----- Audio Clip -----")]
    public AudioClip ChangGun;
    public AudioClip gunFail;
    public AudioClip reload;
    public AudioClip melee;
    public AudioClip[] dash;
    public AudioClip[] walk;
    public AudioClip dodge;
    public AudioClip eCollect;
    public AudioClip heal;
    public AudioClip ultFull;
    public AudioClip useUlt;
    public AudioClip GetHit;


    private void Awake()
    {
        playerSound = this;
    }

    public void Playwalk(int n)
    {
        AudioManager.audioManager.PlaySFX(walk[n]);
    }

    public void Playdash(int n)
    {
        AudioManager.audioManager.PlaySFX(dash[n]);
    }

    public void PlayMelee()
    {
        AudioManager.audioManager.PlaySFX(melee);
    }
}
