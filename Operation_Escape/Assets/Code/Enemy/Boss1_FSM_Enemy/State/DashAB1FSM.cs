using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class DashAB1FSM : BaseState
{
    public DashAB1FSM(FSMBoss1EnemySM stateMachine) : base("DashAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public float stopRadius;
    public bool overdrive;
    public bool pull;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override async void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        cancellationToken = new CancellationTokenSource();
        ai = state.ai;
        overdrive = state.overdrive;
        speed = ai.maxspeed;
        pull = false;
        ai.canMove = true;
        stopRadius = ai.stopRadius;
        if (!overdrive)
        {
            if (!state.rangeAState.rangeAttack)
            {                
                await UniTask.WaitForSeconds(1f);             
                ChangState(state.normalAState);
                return;
            }
            else
            {
                state.rangeAState.rangeAttack = false;
                Attack().Forget();
            }
        }
        else
        {
            Attack().Forget();
        }

        state.normalAState.countATK = 0;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;

        if (pull)
        {
            //PullPlayer();
        }
    }

    public async UniTaskVoid Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;

        ai.canMove = false;
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = ((FSMBoss1EnemySM)stateMachine).boss1AniControl;

        try
        {
            state.isFacing = false;
            ani.ChangeAnimationAttack("StartJump");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ai.canMove = true;
            state.colliderBoss.enabled = false;
            ai.stopRadius = 0.5f;
            if (!overdrive)
            {
                ani.ChangeAnimationAttack("AirJump");
                await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            }
            else
            {
                ani.ChangeAnimationAttack("AirJumpO");
                await UniTask.WaitForSeconds(2f, cancellationToken: token);
            }            
            state.colliderBoss.enabled = true;
            ai.stopRadius = stopRadius;
            ai.canMove = false;
            ani.ChangeAnimationAttack("StopJump");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            await UniTask.WaitForSeconds(2f, cancellationToken: token);

            if (!overdrive)
            {
                state.isFacing = false;
                ani.ChangeAnimationAttack("AfterDash");
                await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);

                ai.canMove = false;
                ani.ChangeAnimationAttack("BossStop");
                await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                ani.animator.SetTrigger("endStop");
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                state.isFacing = true;
                ai.canMove = true;

                ChangState(state.normalAState);
            }
            else
            {

                state.isFacing = false;
                ani.ChangeAnimationAttack("AfterDashO");
                await UniTask.WaitForSeconds(3f, cancellationToken: token);
                PullPlayer().Forget();
                await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);

                ai.canMove = false;
                ani.ChangeAnimationAttack("BossStop");
                await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                ani.animator.SetTrigger("endStop");
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                ai.canMove = true;
                state.isFacing = true;

                if (UnityEngine.Random.value > 0.5f)
                {
                    ChangState(state.normalAState);
                }
                else
                {
                    ChangState(state.rangeAState);
                }
            }

            
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }    

    public async UniTask PullPlayer()
    {
        await UniTask.WaitForSeconds(3);
        float distance = Vector2.Distance(ai.position, ai.targetTransform.position);
        Vector2 dir = (ai.position - ai.targetTransform.position).normalized;
        float range = 30f; // ���з��������֧�ٴ
        float pullStrength = 20f; // ��˹��ç�֧
        if (distance <= range)
        {
            PlayerControl.control.playerMovement.rb.AddForce(dir * pullStrength / distance, ForceMode2D.Force);
        }
    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        cancellationToken?.Cancel();
        state.animator.SetBool("DashAB1FSM", false);
    }
}
