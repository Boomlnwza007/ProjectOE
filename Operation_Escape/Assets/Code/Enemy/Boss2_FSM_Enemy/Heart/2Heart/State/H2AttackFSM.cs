using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H2AttackFSM : BaseState
{
    public H2AttackFSM(FSMHeart2EnemySM stateEnemy) : base("Attack", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    private int countSpike = 0;
    public bool cooldown;

    public override void Enter()
    {
        countSpike++;
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
            state.AttackNSpike(state.timePreSpike);
            await UniTask.WaitForSeconds(state.timePreSpike + 0.5f, cancellationToken: token);
            for (int i = 0; i < 2; i++)
            {
                state.AttackNSpike(0.5f);
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
            }

            if (countSpike >= 3)
            {
                state.AttackRSpike();
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                state.ResetGrid();
                ChangState(state.summon);
                return;
            }
            await UniTask.WaitForSeconds(state.timeCooldownSpike, cancellationToken: token);
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
}
