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
        if (!state.stun)
        {
            ai.canMove = true;
            ai.destination = ai.targetTransform.position;
            distance = Vector2.Distance(ai.position, ai.targetTransform.position);
            if (distance > 10 && !state.cooldownChargeAttack)
            {
                ChangState(state.chargeAttState);
            }
            else if (distance < 3.5 && !state.cooldownSlamAttack)
            {
                ChangState(state.slamAttState);
            }
            else if(state.cooldownSlamAttack)
            {
                state.Movement();
            }
            Debug.Log(state.cooldownSlamAttack);
        }
        else
        {
            var ani = state.animator;
            ai.canMove = false;
            state.timeStun += Time.deltaTime;
            if (state.timeStun > state.timeStunCooldown)
            {
                state.timeStun = 0;
                state.stun = false;
                ai.canMove = true;
                ani.ChangeAnimationAttack("IdleNS");
            }
        }
    }
}
