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
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        ai.canMove = true;
        ai.destination = ai.position;
    }

    public override void UpdateLogic()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        ai.destination = ai.targetTarnsform.position;

        distance = Vector2.Distance(ai.position, ai.targetTarnsform.position);
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
}
