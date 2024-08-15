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
        base.UpdateLogic();
        ai.destination = ai.target.position;

        distance = Vector2.Distance(ai.position, ai.target.position);
        if (distance < 9)
        {
            stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).dashAState);
        }
        else if (distance > 9)
        {
            stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).rangeAState);
        }
    }
}
