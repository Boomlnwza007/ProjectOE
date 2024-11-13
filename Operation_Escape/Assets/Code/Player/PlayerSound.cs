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
    public AudioClip dash;
    public AudioClip walk;
    public AudioClip dodge;
    public AudioClip eCollect;
    public AudioClip heal;
    public AudioClip ultFull;
    public AudioClip useUlt;


    private void Awake()
    {
        playerSound = this;
    }

    public void Playwalk()
    {
        AudioManager.audioManager.PlaySFX(walk);
    }

    public void Playdash()
    {
        AudioManager.audioManager.PlaySFX(dash);
    }

    public void PlayMelee()
    {
        AudioManager.audioManager.PlaySFX(melee);
    }
}
