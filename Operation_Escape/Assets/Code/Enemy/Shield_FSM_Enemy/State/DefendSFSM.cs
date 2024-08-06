using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendSFSM : BaseState
{
    public DefendSFSM(FSMSEnemySM stateMachine) : base("DefendlAttack", stateMachine) { }
    public IAiAvoid ai;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.destination = ai.target.position;
    }

    public override void UpdateLogic()
    {
        ai.destination = ai.target.position;
        ((FSMSEnemySM)stateMachine).canGuard = true;
        float distance = Vector2.Distance(ai.position, ai.target.position);
        if (distance < 8)
        {
            stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
        }
    }
}
