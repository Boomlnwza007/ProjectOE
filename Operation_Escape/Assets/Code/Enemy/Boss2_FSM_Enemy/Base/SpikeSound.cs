using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSound : MonoBehaviour
{
    public AudioSource audioGame;
    [Header("------ Audio Base ------")]
    public AudioClip preSpike;
    public AudioClip spikeAtk;

    public void PlayPreSpike()
    {
        audioGame.PlayOneShot(preSpike);
    }

    public void PlaySpikeAtk()
    {
        audioGame.PlayOneShot(spikeAtk);
    }
}
