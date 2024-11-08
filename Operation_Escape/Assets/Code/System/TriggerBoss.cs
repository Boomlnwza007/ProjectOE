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
        Boss.gameObject.SetActive(false);
        uiBoss.uiBody.SetActive(false);
        Boss.gameObject.SetActive(true);
        Boss.attacking = false;
        Boss1AniControl.boss1AniControl.ResetAnim();
        var state = ((FSMBoss1EnemySM)Boss);
        state.overdriveGage = 0;
        state.overdriveTime = 0;
        state.overdrive = false;
        state.JumpCenter();
        state.ChangState(state.idleState);
    }
}
