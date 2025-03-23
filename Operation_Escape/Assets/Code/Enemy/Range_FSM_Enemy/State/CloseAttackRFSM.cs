using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CloseAttackRFSM : BaseState
{
    public CloseAttackRFSM(FSMREnemySM stateMachine) : base("CloseAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    bool dash;
    private CancellationTokenSource cancellationToken;


    // Start is called before the first frame update
    public override void Enter()
    {
        cancellationToken = new CancellationTokenSource();
        var state = (FSMREnemySM)stateMachine;
        if (state.cooldown)
        {
            stateMachine.ChangState(state.normalAttackState);
            return;
        }

        ai = ((FSMREnemySM)stateMachine).ai;

        Attack(cancellationToken.Token).Forget();

    }
    public override void UpdateLogic()
    {
        if (dash)
        {
            DashStart();
        }
    }

    public async UniTask Attack(CancellationToken token)
    {
        var state = (FSMREnemySM)stateMachine;
        try
        {
            state.animator.isFacing = false;            
            ai.canMove = false;
            Dash();
            await UniTask.WaitUntil(() => !dash, cancellationToken: token);
            ai.canMove = false;
            state.rb.velocity = Vector2.zero;
            await state.PreAttack("PreAttack", 0.1f);
            await state.Attack("Attack", 0.1f);
            state.FireClose();
            await UniTask.WaitForSeconds(0.2f, cancellationToken: token);
            state.animator.ChangeAnimationAttack("Normal");
            ai.canMove = true;

            state.Walk();
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            state.cooldown = true;
            state.animator.isFacing = true;
            ChangState(state.checkDistanceState);
        }
        catch (OperationCanceledException)
        {
            return;
        }        
    }   

    public void Dash()
    {
        var state = ((FSMREnemySM)stateMachine);
        dash = true;
        state.rollSpeed = state.dodgeMaxSpeed;
        state.dashEff.SetActive(true);
    }

    public void DashStart()
    {
        var state = ((FSMREnemySM)stateMachine);
        Vector2 dir = (ai.position - ai.targetTransform.position ).normalized;


        state.rollSpeed -= state.rollSpeed * state.dodgeSpeedDropMultiplier * Time.deltaTime;
        if (state.rollSpeed < state.dodgeMinimium)
        {
            dash = false;
            state.dashEff.SetActive(false);
            state.rb.velocity = Vector3.zero;
        }

        state.rb.velocity = dir * state.rollSpeed;
    }

    public override void Exit()
    {
        cancellationToken?.Cancel();
    }
}
