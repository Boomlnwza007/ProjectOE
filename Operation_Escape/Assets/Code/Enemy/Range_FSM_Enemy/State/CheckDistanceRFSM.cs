using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceRFSM : BaseState
{
    public IAiAvoid ai;
    public float distance;
    public CheckDistanceRFSM(FSMREnemySM stateEnemy) : base("CheckDistance", stateEnemy) { }

    public override void Enter()
    {
        ai = ((FSMREnemySM)stateMachine).ai;
        ai.canMove = true;
        ai.destination = ai.target.position;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;

        distance = Vector2.Distance(ai.position, ai.target.position);
        if (distance < 9)
        {
           stateMachine.ChangState(((FSMREnemySM)stateMachine).closeAttackState);
        }
        else if (distance < 15)
        {
           stateMachine.ChangState(((FSMREnemySM)stateMachine).normalAttackState);
        }
    }
}
