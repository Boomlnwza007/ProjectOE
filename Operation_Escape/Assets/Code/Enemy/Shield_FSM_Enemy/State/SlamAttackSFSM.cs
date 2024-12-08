using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class SlamAttackSFSM : BaseState
{
    public SlamAttackSFSM(FSMSEnemySM stateMachine) : base("slamAttState", stateMachine) { }
    private CancellationTokenSource cancellationToken;
    public IAiAvoid ai;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = state.ai;
        ai.destination = ai.targetTransform.position;
        Attack().Forget();
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMSEnemySM)stateMachine;
        var ani = state.animator;

        try
        {

            state.shield.ShieldIsOn(false);
            ai.canMove = false;
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            Debug.Log("attack");
            await UniTask.WaitForSeconds(state.shield.canGuard ? 2 : 1, cancellationToken: token);

            if (state.shield.canGuard)
            {
                Debug.Log("JumpAttack");
                await UniTask.WaitForSeconds(2, cancellationToken: token);
            }

            state.shield.ShieldIsOn(true);
            state.cooldownSlamAttack = true;
            Cooldown(state.shield.canGuard ? 3 : 2).Forget();

            ChangState(state.checkDistanceState);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTaskVoid Cooldown(float time)
    {
        await UniTask.WaitForSeconds(time);
        var state = (FSMSEnemySM)stateMachine;
        state.cooldownChargeAttack = false;
    }



    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
