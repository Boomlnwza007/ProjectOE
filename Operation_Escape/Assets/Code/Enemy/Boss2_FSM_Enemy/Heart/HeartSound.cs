using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSound : MonoBehaviour
{
    public AudioSource audioGame;
    [Header("------ Audio Base ------")]
    public AudioClip[] preAtk;
    public AudioClip[] monAtk;

    public void PlayPreAtk(int number)
    {
        audioGame.PlayOneShot(preAtk[number]);
    }

    public void PlayMonAtk(int number)
    {
        audioGame.PlayOneShot(monAtk[number]);
    }
}
