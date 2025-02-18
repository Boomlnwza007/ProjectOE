using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H4AttackFSM : BaseState
{
    public H4AttackFSM(FSMHeart4EnemySM stateEnemy) : base("Attack", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    private int count = 0;
    public bool cooldown;

    public override void Enter()
    {
        count++;
        Attack().Forget();
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMHeart4EnemySM)stateMachine;
        //var ani = state.animator;
        
        try
        {
            state.AttackNSpike();
            await UniTask.WaitUntil(() => state.spikeN.final, cancellationToken: token);

            if (count >= 4 && !state.summon.cooldown)
            {
                ChangState(state.summon);
                count = 0;
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
