using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoss : MonoBehaviour
{
    [SerializeField] private StateMachine Boss;
    [SerializeField] private UIBoss uiBoss;
    [SerializeField] private Transform jumpCenter;
    [SerializeField] private AudioClip bgmBoss;
    private bool first;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !first)
        {
            Boss.attacking = true;
            uiBoss.uiBody.SetActive(true);
            AudioManager.audioManager.ChangeBGM(bgmBoss).Forget();
            first = true;
        }
    }
    public void SetUp(StateMachine Boss , UIBoss uiBoss)
    {
        this.Boss = Boss;
        this.uiBoss = uiBoss;
        this.Boss.jumpCenter = jumpCenter;
        AudioManager.audioManager.ChangeBGM(AudioManager.audioManager.backGround,1).Forget();
        first = false;
    }
}
