using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoss : MonoBehaviour
{
    [SerializeField] private StateMachine Boss;
    [SerializeField] private UIBoss uiBoss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Boss.attacking = true;
            uiBoss.uiBody.SetActive(true); 
        }
    }
    public void Off()
    {
        uiBoss.uiBody.SetActive(false);
        Boss.attacking = false;
        ((FSMBoss1EnemySM)Boss).overdriveGage = 0;
    }
}
