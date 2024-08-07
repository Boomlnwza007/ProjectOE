using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMNBFSM : BaseState
{
    public IdleMNBFSM(FSMMiniBossEnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 15f;
    float time;

    public override void Enter()
    {
        ai = ((FSMMEnemySM)stateMachine).ai;
        ai.destination = ai.position;
    }

    public override void UpdateLogic()
    {
        if (Vector2.Distance(ai.position, ai.target.position) < ((FSMMiniBossEnemySM)stateMachine).visRange)
        {
            stateMachine.ChangState(((FSMMEnemySM)stateMachine).CheckDistance);
        }
    }
}
