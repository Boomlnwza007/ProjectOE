using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OverdriveChangFSM : BaseState
{    
    public OverdriveChangFSM(FSMBoss1EnemySM stateEnemy) : base("OverdriveChang", stateEnemy) { }
    public IAiAvoid ai;
    public float distance;
    public float speed;
    public float time;

    public override void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        state.animator.SetBool("OverdriveChangFSM", true);
        ai = state.ai;
        ai.canMove = false;
        time = 0;
        Debug.Log("Start");
    }

    public override void UpdateLogic()
    {
        time += Time.deltaTime;
        if (time > 3)
        {
            ((FSMBoss1EnemySM)stateMachine).JumpCenter();
            Debug.Log("Comple");
            ((FSMBoss1EnemySM)stateMachine).overdrive = true;
            ((FSMBoss1EnemySM)stateMachine).overdriveChang = false;
            stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }
    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        state.animator.SetBool("OverdriveChangFSM", false);
    }
}
