using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoorSound", menuName = "Audio/Door")]
public class DoorSound : ScriptableObject
{
    public AudioClip doorOpen;
    public AudioClip doorClose;
}
