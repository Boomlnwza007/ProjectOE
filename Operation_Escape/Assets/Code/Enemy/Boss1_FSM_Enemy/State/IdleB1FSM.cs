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
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        ai.destination = ai.position;
    }

    public override void UpdateLogic()
    {
        if (Vector2.Distance(ai.position, ai.target.position) < ((FSMBoss1EnemySM)stateMachine).visRange)
        {
            ai.canMove = true;
            stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }
    }
}
