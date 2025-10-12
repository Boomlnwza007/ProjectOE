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

    public override void UpdateLogic()
    {
        var chargeAttack = ((FSMSEnemySM)stateMachine).chargeAttState;
        if (chargeAttack.dash)
        {
            chargeAttack.DashStart();
        }
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMSEnemySM)stateMachine;
        var ani = state.animator;

        try
        {
            ai.monVelocity = Vector2.zero;
            state.shield.ShieldIsOn(false);
            ai.canMove = false;
            //await UniTask.WaitForSeconds(2f, cancellationToken: token);
            ani.ChangeAnimationAttack("GPush");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);

            if (!state.shield.canGuard)
            {
                var chargeAttack = ((FSMSEnemySM)stateMachine).chargeAttState;
                ani.isFacing = true;
                ai.canMove = false;
                ani.animator.speed = 0;
                ani.ChangeAnimationAttack("PreDash");
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                chargeAttack.CheckWay();
                chargeAttack.Dash();
                await UniTask.WaitUntil(() => !chargeAttack.dash, cancellationToken: token);
                state.rb.velocity = Vector2.zero;
                ani.ChangeAnimationAttack("Dash");
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                ani.isFacing = true;
            }

            ani.ChangeAnimationAttack("IdleNS");
            await UniTask.WaitForSeconds(2, cancellationToken: token);
            state.shield.ShieldIsOn(true);
            Cooldown(state.shield.canGuard ? 4 : 2).Forget();
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
        var state = (FSMSEnemySM)stateMachine;
        state.cooldownSlamAttack = true;
        await UniTask.WaitForSeconds(time);
        state.cooldownSlamAttack = false;
    }   

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
