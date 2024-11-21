using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCharacter : MonoBehaviour
{
    public AudioSource audioGame;
    [Header("------ Audio Base ------")]
    public AudioClip walk;
    public AudioClip[] attack;

    public void PlayWalk()
    {
        audioGame.PlayOneShot(walk);
    }

    public void PlayAtttack(int index)
    {
        audioGame.PlayOneShot(attack[index]);
    }

}
