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
    private int countSpike = 0;
    public bool cooldown;

    public override void Enter()
    {
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
            countSpike++;
            state.AttackZSpike();
            await UniTask.WaitForSeconds(2.5f, cancellationToken: token);
            if (countSpike > 1)
            {
                if (state.summon.cooldown)
                {
                    countSpike = 0;
                    ChangState(state.summon);
                    return;
                }
            }

            await UniTask.WaitForSeconds(state.timeCooldownSpike);
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
