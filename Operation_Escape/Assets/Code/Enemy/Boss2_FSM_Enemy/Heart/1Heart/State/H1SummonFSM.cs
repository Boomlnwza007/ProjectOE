using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H1SummonFSM : BaseState
{
    public H1SummonFSM(FSMHeart1EnemySM stateEnemy) : base("Summon", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    public bool cooldown;

    public override void Enter()
    {
        var state = (FSMHeart1EnemySM)stateMachine;
        Attack().Forget();
        state.shield.ShieldIsOn(false);
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMHeart1EnemySM)stateMachine;
        //var ani = state.animator;

        try
        {
            for (int i = 0; i < 3; i++)
            {
                state.SummonMinion(0);
            }
            state.ResetPositionsMInion();
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            Cooldown().Forget();

            await UniTask.WaitForSeconds(state.timeCooldownSpike);
            state.shield.ShieldIsOn(true);

            ChangState(state.attack);
        }
        catch (System.OperationCanceledException)
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
        var state = (FSMHeart1EnemySM)stateMachine;
        cooldown = true;
        await UniTask.WaitForSeconds(state.timeCooldownMinion);
        cooldown = false;
    }
}
