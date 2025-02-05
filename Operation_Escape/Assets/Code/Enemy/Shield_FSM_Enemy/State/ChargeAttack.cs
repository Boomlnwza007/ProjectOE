using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class ChargeAttack : BaseState
{
    public ChargeAttack(FSMSEnemySM stateMachine) : base("chargeAttState", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    private float time;
    private bool first;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.destination = ai.targetTransform.position;
        Attack().Forget();
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMSEnemySM)stateMachine;
        var ani = state.animator;


        time = 0;

        try
        {
            state.shield.ShieldIsOn(false);
            ai.canMove = false;
            //ani.isFacing = false;
            ani.animator.speed = 0;
            ani.ChangeAnimationAttack("PreDash");
            ani.isFacing = false;
            await UniTask.WaitForSeconds(2f , cancellationToken: token);
            ai.destination = CalculateDestination(ai.position, ai.targetTransform.position, state.jumpLength, state.raycastMaskWay);
            await Charge();
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            ani.isFacing = true;
            ani.ChangeAnimationAttack("IdleNS");
            await UniTask.DelayFrame(1);


            if (!state.shield.canGuard)
            {
                ai.canMove = false;
                //ani.isFacing = false;
                ani.animator.speed = 0;
                ani.ChangeAnimationAttack("PreDash");
                ani.isFacing = false;
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                ai.destination = CalculateDestination(ai.position, ai.targetTransform.position, state.jumpLength, state.raycastMaskWay);
                await Charge();
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                ani.isFacing = true;
            }

            ani.animator.speed = 1;
            state.Walk();
            ai.canMove = false;
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            ani.ChangeAnimationAttack("IdleNS");
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            state.shield.ShieldIsOn(true);
            float timeCooldown = state.shield.canGuard ? 4 : 2;
            Cooldown(timeCooldown).Forget();
            ChangState(state.checkDistanceState);

        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public Vector2 CalculateDestination(Vector2 currentPosition, Vector2 targetPosition, float jumpLength, LayerMask mask)
    {
        Vector2 direction = (targetPosition - currentPosition).normalized;
        RaycastHit2D[] raycast = Physics2D.RaycastAll(currentPosition, direction, jumpLength, mask);

        foreach (var hit in raycast)
        {
            if (hit.collider != null && hit.collider.gameObject != stateMachine.gameObject)
            {
                return hit.point;
            }
        }

        Debug.DrawLine(currentPosition, currentPosition + (direction * jumpLength), Color.green, 2f);
        return currentPosition + (direction * jumpLength);
    }

    public async UniTask Charge()
    {
        var token = cancellationToken.Token;
        var state = (FSMSEnemySM)stateMachine;
        bool hasAttacked = false;
        var ani = state.animator;     
        
        state.Walk();
        ai.canMove = true;
        state.Run(5);
        ani.animator.speed = 1;

        while (time < 10 && !hasAttacked)//Edit Time Run 
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.destination, ai.position) < 2f && ai.endMove)
            {
                //Debug.Log("ai.endMove");

                ai.canMove = false;
                ai.monVelocity = Vector2.zero;
                state.Walk();
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

                    ai.canMove = false;
                    ai.monVelocity = Vector2.zero;
                    state.Walk();
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

    public async UniTaskVoid Cooldown(float time)
    {
        var state = (FSMSEnemySM)stateMachine;
        state.cooldownChargeAttack = true;
        await UniTask.WaitForSeconds(time); 
        state.cooldownChargeAttack = false;
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }

}
