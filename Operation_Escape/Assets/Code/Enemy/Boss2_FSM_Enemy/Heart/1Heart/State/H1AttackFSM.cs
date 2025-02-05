using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H1AttackFSM : BaseState
{
    public H1AttackFSM(FSMHeart1EnemySM stateEnemy) : base("Attack", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    public int count = 0;

    public override void Enter()
    {
        Attack().Forget();
        count++;
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMHeart1EnemySM)stateMachine;

        try
        {
            state.AttackNSpike(new Vector2Int(2,2));
            await UniTask.WaitUntil(() => state.spikeN.final, cancellationToken: token);

            if (count>=4)
            {                
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
