using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnemy_Boss1 : AudioCharacter
{
    [Header("------ Audio Base ------")]
    public AudioClip[] effect;

    public void PlayWarp(int index)
    {
        audioGame.PlaySFX(effect[index]);
    }
}
