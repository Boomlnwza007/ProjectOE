using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBGM : MonoBehaviour
{
    public AudioClip backGround;
    public float fadeDuration = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.audioManager.ChangeBGM(backGround, fadeDuration).Forget();
        Destroy(gameObject);
    }
}
