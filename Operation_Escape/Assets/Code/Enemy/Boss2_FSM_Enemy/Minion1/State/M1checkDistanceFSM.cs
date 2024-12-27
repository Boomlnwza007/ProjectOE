using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class M1checkDistanceFSM : BaseState
{
    public M1checkDistanceFSM(FSMMinion1EnemySM stateMachine) : base("Attacck", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMMinion1EnemySM)stateMachine).ai;
        ai.canMove = true;
        ai.destination = ai.targetTransform.position;
    }

    public override void UpdateLogic()
    {
        float distance = Vector2.Distance(ai.position, ai.targetTransform.position);
        if (distance < 5)
        {
            stateMachine.ChangState(((FSMMinion1EnemySM)stateMachine).attack);
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
