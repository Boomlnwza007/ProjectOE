using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnemy_RE : AudioCharacter
{
    [Header("------ Audio Special ------")]
    public AudioClip dodge;

    public void PlayDodge()
    {
        audioGame.PlayOneShot(dodge);
    }
}
