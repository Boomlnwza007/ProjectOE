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
            countSpike++;
            state.AttackRSpike();
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            Debug.Log(countSpike);
            if (countSpike > 3)
            {
                if (!state.summon.cooldown)
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
