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
        var state = (FSMSEnemySM)stateMachine;
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.canMove = true;
        ai.destination = ai.targetTransform.position;
        if (!state.shield.canGuard)
        {
            ChangState(state.NoShieldState);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;
        var state = (FSMSEnemySM)stateMachine;
        distance = Vector2.Distance(ai.position, ai.targetTransform.position);
        if (distance > 10)
        {
            ChangState(state.defendState);
        }
        else if (distance < 5)
        {
            ChangState(state.defendAttState);
        }
    }
}
