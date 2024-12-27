
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H2SummonFSM : BaseState
{
    public H2SummonFSM(FSMHeart2EnemySM stateEnemy) : base("Attack", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    public bool cooldown;

    public override void Enter()
    {
        Attack().Forget();
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMHeart2EnemySM)stateMachine;
        //var ani = state.animator;

        try
        {
            state.AttackRSpike();
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            state.SummonMinion(0);
            state.ResetPositions();
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            Cooldown().Forget();
            ChangState(state.attack);
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    public override void Exit()
    {
        cancellationToken.Cancel();
    }

    public async UniTask Cooldown()
    {
        var state = (FSMHeart2EnemySM)stateMachine;
        cooldown = true;
        await UniTask.WaitForSeconds(state.timeCooldownMinion);
        cooldown = false;
    }
}
