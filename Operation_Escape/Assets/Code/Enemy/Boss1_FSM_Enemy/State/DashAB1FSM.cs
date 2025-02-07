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

                state.isFacing = true;
                ani.ChangeAnimationAttack("Wait");
                await UniTask.WaitForSeconds(3f, cancellationToken: token);
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

                state.isFacing = true;
                ani.ChangeAnimationAttack("Wait");
                await UniTask.WaitForSeconds(3f, cancellationToken: token);
                ai.canMove = true;

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
        float range = 30f; // ระยะที่เริ่มดึงดูด
        float pullStrength = 20f; // กำหนดแรงดึง
        if (distance <= range)
        {
            PlayerControl.control.playerMovement.rb.AddForce(dir * pullStrength / distance, ForceMode2D.Force);
        }
    }
      
    //public void ChangState(BaseState Nextstate)
    //{
    //    var state = (FSMBoss1EnemySM)stateMachine;
    //    if (!state.attacking)
    //    {
    //        state.JumpCenter();
    //        state.ChangState(state.idleState);
    //        return;

    //    }

    //    if (state.overdriveChang)
    //    {
    //        state.ChangState(state.overdriveChangState);
    //    }
    //    else
    //    {
    //        state.ChangState(Nextstate);
    //    }
    //}

    public async UniTask LaserFollowIn()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        //Vector2 directionToPlayer = (ai.targetTarnsform.position - ai.position).normalized;
        //float angleDirectionToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        //float angleLeft = angleDirectionToPlayer + 90;
        //float angleRight = angleDirectionToPlayer - 90;
        //float[] angle = { angleLeft, angleRight };

        //state.CreatLaserGun(angle);
        await UniTask.WhenAll(state.ShootLaserFollowIn(2f, 3f, 1, 4.5f), state.RangeFollow(2f));
    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        cancellationToken?.Cancel();
        state.animator.SetBool("DashAB1FSM", false);
    }
}
