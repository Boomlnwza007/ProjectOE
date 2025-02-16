using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H2SummonFSM : BaseState
{
    public H2SummonFSM(FSMHeart2EnemySM stateEnemy) : base("Summon", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    public bool cooldown;

    public override void Enter()
    {
        var state = (FSMHeart2EnemySM)stateMachine;
        Attack().Forget();
        state.shield.ShieldIsOn(false);
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMHeart2EnemySM)stateMachine;
        var ani = state.animator;

        try
        {

            state.animator.ChangeAnimationAttack("ExpandAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            Cooldown().Forget();
            state.shield.ShieldIsOn(true);
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
