using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCharacter : MonoBehaviour
{
    protected AudioManager audioGame;
    [Header("------ Audio Base ------")]
    public AudioClip walk;
    public AudioClip[] attack;

    private void Awake()
    {
        audioGame = AudioManager.audioManager;
    }

    public void PlayWalk()
    {
        audioGame.PlaySFX(walk);
    }

    public void PlayAtttack(int index)
    {
        audioGame.PlaySFX(attack[index]);
    }

}
