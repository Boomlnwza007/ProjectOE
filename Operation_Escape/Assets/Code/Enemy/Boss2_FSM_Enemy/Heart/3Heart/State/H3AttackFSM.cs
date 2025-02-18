using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H3AttackFSM : BaseState
{
    public H3AttackFSM(FSMHeart3EnemySM stateEnemy) : base("Attack", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    private int countSpike = 0;

    public override void Enter()
    {
        countSpike++;
        Attack().Forget();
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMHeart3EnemySM)stateMachine;
        //var ani = state.animator;
        
        try
        {
            state.AttackNSpike();
            await UniTask.WaitUntil(() => state.spikeN.final, cancellationToken: token);

            if (countSpike >= 4 && !state.summon.cooldown)
            {
                ChangState(state.summon);
                countSpike = 0;
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
