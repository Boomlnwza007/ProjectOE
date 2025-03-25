using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


public class ChargeAttack : BaseState
{
    public ChargeAttack(FSMSEnemySM stateMachine) : base("chargeAttState", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    public Vector3 target;
    public bool dash;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.destination = ai.targetTransform.position;
        if (!CheckWay())
        {
            ChangState(state.checkDistanceState);
        }
        else
        {
            ai.canMove = false;
            dash = false;
            Attack().Forget();
        }
    }

    public bool CheckWay()
    {
        var state = (FSMSEnemySM)stateMachine;
        Vector2 dir = (ai.targetTransform.position - ai.position).normalized;
        RaycastHit2D raycast = Physics2D.Raycast(ai.position, dir, state.jumpLength, state.raycastMaskWalk);
        if (raycast.collider != null && raycast.collider.CompareTag("Player"))
        {
            target = ai.targetTransform.position;
        }
        return raycast.collider != null && raycast.collider.CompareTag("Player");
    }

    public override void UpdateLogic()
    {
        if (dash)
        {
            DashStart();
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
            state.shield.ShieldIsOn(false);
            ani.animator.speed = 0;
            ani.ChangeAnimationAttack("PreDash");
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            CheckWay();
            Dash();
            await UniTask.WaitUntil(() => !dash, cancellationToken: token);
            state.rb.velocity = Vector2.zero; 
            ani.ChangeAnimationAttack("Dash");
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);

            if (!state.shield.canGuard)
            {
                ani.isFacing = true;
                ani.animator.speed = 0;
                ani.ChangeAnimationAttack("PreDash");
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                CheckWay();
                Dash();
                await UniTask.WaitUntil(() => !dash, cancellationToken: token);
                state.rb.velocity = Vector2.zero;
                ani.ChangeAnimationAttack("Dash");
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            }

            ani.ChangeAnimationAttack("IdleNS");
            ani.isFacing = true;
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            Cooldown(state.shield.canGuard ? 4 : 2).Forget();
            state.shield.ShieldIsOn(true);
            ai.canMove = true;
            ChangState(state.checkDistanceState);

        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public void DashStart()
    {
        var state = ((FSMSEnemySM)stateMachine);
        Vector2 dir = (target - ai.position).normalized;
        Collider2D[] raycastCircle = Physics2D.OverlapCircleAll(ai.position, 1.5f, state.raycastMask);
        RaycastHit2D[] raycast = Physics2D.RaycastAll(ai.position, dir, state.dodgeStopRange, state.raycastMaskWay);
        if (raycast.Length > 0 || raycastCircle.Any(item => item.gameObject != state.gameObject))
        {
            state.rollSpeed = state.dodgeMinimium;
            dash = false;
            state.rb.velocity = Vector2.zero;
        }

        state.rollSpeed -= state.rollSpeed * state.dodgeSpeedDropMultiplier * Time.deltaTime;
        if (state.rollSpeed < state.dodgeMinimium)
        {
            dash = false;
            state.rb.velocity = Vector3.zero;
        }

        state.rb.velocity = dir * state.rollSpeed;
    }

    public void Dash()
    {
        var state = ((FSMSEnemySM)stateMachine);
        state.animator.isFacing = false;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        bool isFacingRight = angle > -90 && angle < 90;
        state.animator.animator.SetBool("IsRight", isFacingRight);
        state.animator.animator.SetFloat("horizon", isFacingRight ? 1 : -1);
        dash = true;
        state.rollSpeed = state.dodgeMaxSpeed;
        state.animator.animator.speed = 1;
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
