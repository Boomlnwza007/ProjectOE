using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class NormalAttackEMFSM : BaseState
{
    public NormalAttackEMFSM(FSMMEnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationTokenSource;
    bool dash;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMMEnemySM)stateMachine).ai;
        ((FSMMEnemySM)stateMachine).Walk();
        cancellationTokenSource = new CancellationTokenSource();

        Attack(cancellationTokenSource.Token).Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;
        if (dash)
        {
            DashStart();
        }
    }

    public async UniTaskVoid Attack(CancellationToken token)
    {
        var state = ((FSMMEnemySM)stateMachine);
        try
        {
            state.Run(0.2f);

            ai.canMove = false;
            state.animator.isFacing = false;
            await state.PreAttack("PreAttack", 0.5f);
            Dash();
            await state.Attack("Attack", 0.3f);
            state.rb.velocity = Vector2.zero;
            if (Distance())
            {
                state.animator.ChangeAnimationAttack("Normal");
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                ai.canMove = true;
                state.animator.isFacing = true;
                state.Walk();
                ChangState(state.CheckDistance);
                return;
            }

            ai.canMove = true;
            state.animator.isFacing = true;
            await UniTask.DelayFrame(1);

            ai.canMove = false;
            state.animator.isFacing = false;
            await state.PreAttack("PreAttack", 0.8f);
            Dash();
            await state.Attack("Attack", 0.3f);
            state.rb.velocity = Vector2.zero;
            if (Distance())
            {
                state.animator.ChangeAnimationAttack("Normal");
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                ai.canMove = true;
                state.animator.isFacing = true;
                state.Walk();
                ChangState(state.CheckDistance);
                return;
            }

            ai.canMove = true;
            state.animator.isFacing = true;
            await UniTask.DelayFrame(1);

            ai.canMove = false;
            state.animator.isFacing = false;
            await state.PreAttack("PreAttack", 1.5f);
            Dash();
            await state.Attack("Attack", 0.3f);
            state.rb.velocity = Vector2.zero;

            state.animator.ChangeAnimationAttack("Tired");
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            state.animator.ChangeAnimationAttack("Normal");
            state.animator.isFacing = true;
            ai.canMove = true;
            state.Walk();
            ChangState(state.CheckDistance);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("The attack was canceled.");
            return;
        }

    }

    public bool Distance()
    {
        if (Vector2.Distance(ai.targetTransform.position, ai.position) > 3f)
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
         cancellationTokenSource.Cancel();
    }

    public async UniTask Attack(float tPreA ,float tA)
    {
        var state = ((FSMMEnemySM)stateMachine);
        ai.canMove = false;
        state.animator.isFacing = false;
        await state.PreAttack("PreAttack", tPreA);
        Dash();
        await state.Attack("Attack", tA);
        state.rb.velocity = Vector2.zero;
        if (Distance())
        {
            state.animator.ChangeAnimationAttack("Normal");
            await UniTask.WaitForSeconds(1f); Debug.Log("Far");
            ai.canMove = true;
            state.animator.isFacing = true;
            state.Walk();
            ChangState(state.CheckDistance);
            return;
        }

        ai.canMove = true;
        state.animator.isFacing = true;
        await UniTask.DelayFrame(1);
    }

    public void Dash()
    {
        var state = ((FSMMEnemySM)stateMachine);
        dash = true;
        state.rollSpeed = state.dodgeMaxSpeed;
    }

    public void DashStart()
    {
        var state = ((FSMMEnemySM)stateMachine);
        Vector2 dir = Vector2.zero;
        if (state.animator.isFacingRight)
        {
            dir = Vector2.right;
        }
        else
        {
            dir = Vector2.left;
        }
        //Vector2 dir = (ai.targetTransform.position - ai.position).normalized;

        RaycastHit2D[] raycast = Physics2D.RaycastAll(ai.position, dir, state.dodgeStopRange, LayerMask.GetMask("Obstacle"));
        if (raycast.Length > 0)
        {
            state.rollSpeed = state.dodgeMinimium;
            dash = false;
            state.rb.velocity = Vector3.zero;
            return;
        }
        state.rollSpeed -= state.rollSpeed * state.dodgeSpeedDropMultiplier * Time.deltaTime;
        if (state.rollSpeed < state.dodgeMinimium)
        {
            dash = false;
            state.rb.velocity = Vector3.zero;
        }

        state.rb.velocity = dir * state.rollSpeed;
    }

}
