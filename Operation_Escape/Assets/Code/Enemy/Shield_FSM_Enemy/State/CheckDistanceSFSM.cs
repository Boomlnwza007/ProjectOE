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
        ai.canMove = true;
        ai.destination = ai.targetTarnsform.position;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTarnsform.position;

        distance = Vector2.Distance(ai.position, ai.targetTarnsform.position);
        if (distance < 2)
        {
            stateMachine.ChangState(((FSMSEnemySM)stateMachine).bashState);
        }
        else if (distance < 8)
        {
            stateMachine.ChangState(((FSMSEnemySM)stateMachine).chargState);
        }
        else if (distance < 15)
        {
            stateMachine.ChangState(((FSMSEnemySM)stateMachine).defendState);
        }
    }
}
