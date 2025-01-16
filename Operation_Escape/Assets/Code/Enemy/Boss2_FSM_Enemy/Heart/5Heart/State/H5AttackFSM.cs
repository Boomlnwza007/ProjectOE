using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H5AttackFSM : BaseState
{
    public H5AttackFSM(FSMHeart5EnemySM stateEnemy) : base("Attack", stateEnemy) { }
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
        var state = (FSMHeart5EnemySM)stateMachine;
        //var ani = state.animator;
        
        try
        {
            state.AttackNSpike(state.timePreSpike);
            await UniTask.WaitForSeconds(state.timePreSpike + 0.5f, cancellationToken: token);
            for (int i = 0; i < 3; i++)
            {
                state.AttackNSpike(0.5f);
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
            }

            state.AttackRSpike(0.5f);
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            state.AttackRSpike(0.5f);
            await UniTask.WaitForSeconds(1f, cancellationToken: token);

            if (countSpike >= 5)
            {
                state.AttackZSpike();
                await UniTask.WaitForSeconds(2f, cancellationToken: token);
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
