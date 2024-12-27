using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class M3AttackFSM : BaseState
{
    public M3AttackFSM(FSMMinion3EnemySM stateMachine) : base("Attacck", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMMinion3EnemySM)stateMachine).ai;
        Attack().Forget();
    }

    public async UniTaskVoid Attack() // Pass the CancellationToken
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMMinion3EnemySM)stateMachine;
        //var ani = state.animator;

        try
        {
            Debug.Log("Attack");
            await UniTask.WaitForSeconds(0.5f);
            ChangState(state.checkDistance);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
