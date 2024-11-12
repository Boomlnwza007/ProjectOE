using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoss : MonoBehaviour
{
    [SerializeField] private StateMachine Boss;
    [SerializeField] private UIBoss uiBoss;
    [SerializeField] private Transform jumpCenter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Boss.attacking = true;
            uiBoss.uiBody.SetActive(true); 
        }
    }
    public void SetUp(StateMachine Boss , UIBoss uiBoss)
    {
        this.Boss = Boss;
        this.uiBoss = uiBoss;
        this.Boss.jumpCenter = jumpCenter;
    }
}
