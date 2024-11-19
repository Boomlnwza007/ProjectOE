using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnemy_EM : AudioCharacter
{
    [Header("------ Audio Base ------")]
    public AudioClip jump;

    public void PlayJump(string name)
    {
        audioGame.PlaySFX(jump);
    }
}
