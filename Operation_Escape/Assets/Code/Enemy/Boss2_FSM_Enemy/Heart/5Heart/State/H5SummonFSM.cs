
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H5SummonFSM : BaseState
{
    public H5SummonFSM(FSMHeart5EnemySM stateEnemy) : base("Summon", stateEnemy) { }
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
        var state = (FSMHeart5EnemySM)stateMachine;
        //var ani = state.animator;

        try
        {
            for (int i = 0; i < 5; i++)
            {
                state.SummonMinion(0,new Vector2Int(2,2));
            }
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
        var state = (FSMHeart5EnemySM)stateMachine;
        cooldown = true;
        await UniTask.WaitForSeconds(state.timeCooldownMinion);
        cooldown = false;
    }
}
