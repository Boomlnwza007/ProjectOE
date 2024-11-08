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
        var state = ((FSMBoss1EnemySM)Boss);
        state.isFacing = true;
        Boss1AniControl.boss1AniControl.ChangeAnimationState("Wait");
        Boss1AniControl.boss1AniControl.ResetAnim();
        state.overdriveGage = 0;
        state.overdriveTime = 0;
        state.overdrive = false;
        state.rb.velocity = Vector3.zero;
        state.JumpCenter();
        state.ChangState(state.idleState);
        Boss1AniControl.boss1AniControl.ResetAnim();

    }
}
