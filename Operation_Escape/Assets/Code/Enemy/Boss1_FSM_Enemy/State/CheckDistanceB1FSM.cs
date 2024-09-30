using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceB1FSM : BaseState
{
    public IAiAvoid ai;
    public float distance;
    public CheckDistanceB1FSM(FSMBoss1EnemySM stateEnemy) : base("CheckDistance", stateEnemy) { }

    public override void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        state.animator.SetBool("CheckDistanceB1FSM", true);
        ai = state.ai;
        ai.canMove = true;
        ai.destination = ai.position;
    }

    public override void UpdateLogic()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        ai.destination = ai.targetTransform.position;
        distance = Vector2.Distance(ai.position, ai.targetTransform.position);
        if (distance < 9)
        {
            ChangState(state.dashAState);
        }
        else if (distance > 9)
        {
            ChangState(state.rangeAState);
        }
    }

    public void ChangState(BaseState Nextstate)
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        if (state.overdriveChang)
        {
            state.ChangState(state.overdriveChangState);
        }
        else
        {
            state.ChangState(Nextstate);
        }
    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        state.animator.SetBool("CheckDistanceB1FSM", false);
    }
}
