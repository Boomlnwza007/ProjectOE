using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnemy_EM : AudioCharacter
{
    [Header("------ Audio Special ------")]
    public AudioClip jump;

    public void PlayJump()
    {
        audioGame.PlayOneShot(jump);
    }
}
