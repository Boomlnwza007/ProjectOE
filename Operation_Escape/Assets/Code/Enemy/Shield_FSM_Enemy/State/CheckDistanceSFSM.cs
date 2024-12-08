using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceSFSM : BaseState
{
    public IAiAvoid ai;
    public float distance;
    public CheckDistanceSFSM(FSMSEnemySM stateEnemy) : base("CheckDistance", stateEnemy) { }

    public override void Enter()
    {
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.destination = ai.targetTransform.position;
        var state = (FSMSEnemySM)stateMachine;

        if (state.shield.canGuard)
        {
            state.shield.redy = true;
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        var state = (FSMSEnemySM)stateMachine;
        if (state.shield.canGuard)
        {
            ai.canMove = true;
            ai.destination = ai.targetTransform.position;
            distance = Vector2.Distance(ai.position, ai.targetTransform.position);
            if (distance > 10 && !state.cooldownChargeAttack)
            {
                ChangState(state.chargeAttState);
            }
            else if (distance < 5 && !state.cooldownSlamAttack)
            {
                ChangState(state.slamAttState);
            }
        }
        else
        {
            ai.canMove = false;
        }
    }
}
