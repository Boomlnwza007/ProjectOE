using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1AniControl : MonoBehaviour
{
    [SerializeField] private FSMBoss1EnemySM boss1;
    [SerializeField] private Animator animator;

    private void Start()
    {
        //boss1.curState.nameState;
    }

    private void Update()
    {
        switch (boss1.curState)
        {
            case CheckDistanceB1FSM:
                animator.SetBool("CheckDistanceB1FSM",true);

                break;
            case DashAB1FSM:
                animator.SetBool("DashAB1FSM", true);

                break;
            case IdleB1FSM:
                animator.SetBool("IdleB1FSM", true);

                break;
            case NormalAB1FSM:
                animator.SetBool("NormalAB1FSM", true);

                break;
            case RangeAB1Fsm:
                animator.SetBool("RangeAB1Fsm", true);

                break;
            case OverdriveChangFSM:
                animator.SetBool("OverdriveChangFSM", true);

                break;
        }
    }

    public void Set()
    {
        animator.SetBool("CheckDistanceB1FSM", false);
        animator.SetBool("DashAB1FSM", false);
        animator.SetBool("IdleB1FSM", false);
        animator.SetBool("NormalAB1FSM", false);
        animator.SetBool("RangeAB1Fsm", false);
        animator.SetBool("OverdriveChangFSM", false);
    }

}
