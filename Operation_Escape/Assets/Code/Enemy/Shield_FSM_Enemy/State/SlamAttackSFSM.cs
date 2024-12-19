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
            ai.monVelocity = Vector2.zero;
            state.shield.ShieldIsOn(false);
            ai.canMove = false;
            //await UniTask.WaitForSeconds(2f, cancellationToken: token);
            ani.ChangeAnimationAttack("GPush");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("IdleNS");
            await UniTask.WaitForSeconds(state.shield.canGuard ? 2 : 1, cancellationToken: token);

            if (!state.shield.canGuard)
            {
                ai.canMove = false;
                ani.isFacing = false;
                ai.destination = state.chargeAttState.CalculateDestination(ai.position, ai.targetTransform.position, state.jumpLength, state.raycastMaskWay);
                ani.animator.speed = 0;
                ani.ChangeAnimationAttack("PreDash");
                await UniTask.WaitForSeconds(1f, cancellationToken: token);

                await Charge();

                await UniTask.WaitForSeconds(1, cancellationToken: token);
                ani.ChangeAnimationAttack("IdleNS");
                ani.isFacing = true;
                await UniTask.WaitForSeconds(1, cancellationToken: token);
            }

            state.shield.ShieldIsOn(true);
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
        var state = (FSMSEnemySM)stateMachine;
        state.cooldownSlamAttack = true;
        await UniTask.WaitForSeconds(time);
        state.cooldownSlamAttack = false;
    }

    public async UniTask Charge()
    {
        var token = cancellationToken.Token;
        var state = (FSMSEnemySM)stateMachine;
        bool hasAttacked = false;
        var ani = state.animator;
        float time = 0;

        state.Walk();
        ai.canMove = true;
        state.Run(5);
        ani.animator.speed = 1;

        while (time < 10 && !hasAttacked)//Edit Time Run 
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.destination, ai.position) < 3f && ai.endMove)
            {
                //Debug.Log("ai.endMove");
                ani.ChangeAnimationAttack("Dash");
                Debug.Log("Attack");
                hasAttacked = true;
                //state.animator.isFacing = true;
                break;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(ai.position, 2f, state.raycastMask);
            foreach (var hit in colliders)
            {
                if (hit.gameObject != state.gameObject)
                {
                    Debug.Log(hit.name + "hit 2 ");
                    ani.ChangeAnimationAttack("Dash");
                    Debug.Log("Attack");
                    hasAttacked = true;
                    //state.animator.isFacing = true;
                    break;
                }
            }
            token.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }

    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
