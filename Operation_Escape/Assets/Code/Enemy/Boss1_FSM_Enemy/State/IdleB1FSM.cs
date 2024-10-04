using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleB1FSM : BaseState
{
    public IdleB1FSM(FSMBoss1EnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 15f;

    public override void Enter()
    {
        ((FSMBoss1EnemySM)stateMachine).animator.SetBool("IdleB1FSM", true);
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        ai.destination = ai.position;
        ai.canMove = false;
    }

    public override void UpdateLogic()
    {

        if (Vector2.Distance(ai.position, ai.targetTransform.position) < ((FSMBoss1EnemySM)stateMachine).visRange && ((FSMBoss1EnemySM)stateMachine).attacking)
        {
            ai.canMove = true;
            stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }
    }

    public override void Exit()
    {
        ((FSMBoss1EnemySM)stateMachine).animator.SetBool("IdleB1FSM", false);
    }
}
