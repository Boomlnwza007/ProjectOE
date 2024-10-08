using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceEMFSM : BaseState
{
    public IAiAvoid ai;
    public float distance;
    public CheckDistanceEMFSM(FSMMEnemySM stateEnemy) : base("CheckDistance", stateEnemy) { }

    public override void Enter()
    {
        ai = ((FSMMEnemySM)stateMachine).ai;
        ai.canMove = true;
        ai.destination = ai.position;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        var state = (FSMMEnemySM)stateMachine;
        ai.destination = ai.targetTransform.position;

        distance = Vector2.Distance(ai.position, ai.targetTransform.position);
        if (distance < 2)
        {
            stateMachine.ChangState(state.N1Attack);
        }
        else if (distance >= 5 && !state.cooldown)  
        {
            stateMachine.ChangState(state.Charge);
        }
    }

}
