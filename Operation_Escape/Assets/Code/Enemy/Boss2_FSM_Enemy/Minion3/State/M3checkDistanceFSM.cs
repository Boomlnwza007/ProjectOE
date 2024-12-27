using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class M3checkDistanceFSM : BaseState
{
    public M3checkDistanceFSM(FSMMinion3EnemySM stateMachine) : base("Attacck", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMMinion3EnemySM)stateMachine).ai;
        ai.canMove = true;
        ai.destination = ai.targetTransform.position;
    }

    public override void UpdateLogic()
    {
        float distance = Vector2.Distance(ai.position, ai.targetTransform.position);
        if (distance < 2)
        {
            stateMachine.ChangState(((FSMMinion3EnemySM)stateMachine).attack);
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
