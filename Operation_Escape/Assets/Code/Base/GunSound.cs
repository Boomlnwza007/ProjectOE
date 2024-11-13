using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GunName",menuName = "Audio/Gun")]
public class GunSound : ScriptableObject
{
    public AudioClip shoot;
    public AudioClip shootUltimate;
    public AudioClip[] special;
}
