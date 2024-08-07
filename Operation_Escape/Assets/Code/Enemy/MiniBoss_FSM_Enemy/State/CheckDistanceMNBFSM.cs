using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceMNBFSM : BaseState
{
    public IAiAvoid ai;
    public float distance;
    public CheckDistanceMNBFSM(FSMMiniBossEnemySM stateEnemy) : base("CheckDistance", stateEnemy) { }

    public override void Enter()
    {
        ai = ((FSMMEnemySM)stateMachine).ai;
        ai.canMove = true;
        ai.destination = ai.position;

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;

        distance = Vector2.Distance(ai.position, ai.target.position);
        if (distance < 2)
        {
            //stateMachine.ChangState(((FSMMiniBossEnemySM)stateMachine).N1Attack);
        }
        else if (distance < 5)
        {
            //stateMachine.ChangState(((FSMMiniBossEnemySM)stateMachine).Charge);
        }
    }
}
