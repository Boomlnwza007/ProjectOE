using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnemy_SE : AudioCharacter
{
    [Header("------ Audio Special ------")]
    public AudioClip charge;

    public void PlayCharge()
    {
        audioGame.PlayOneShot(charge);
    }
}
