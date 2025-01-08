using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class NormalAB1FSM : BaseState
{
    public NormalAB1FSM(FSMBoss1EnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    private CancellationTokenSource cancellationToken;
    public IAiAvoid ai;
    public float speed;
    public float Charg;
    public float runTime;
    public bool overdrive;
    bool dash;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        cancellationToken = new CancellationTokenSource();
        state.animator.SetBool("NormalAB1FSM", true);
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        overdrive = ((FSMBoss1EnemySM)stateMachine).overdrive;
        ai.canMove = true;
        speed = ai.maxspeed;
        if (!overdrive)
        {
            runTime = 4f;
        }
        else
        {
            runTime = 3f;
        }

        Attack().Forget();

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;
    }

    public override void UpdatePhysics()
    {
        if (dash)
        {
            DashStart();
        }
    }

    public async UniTaskVoid Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = ((FSMBoss1EnemySM)stateMachine).boss1AniControl;
        var token = cancellationToken.Token;
        try
        {
            await UniTask.WaitForSeconds(1, cancellationToken: token);
            ai.maxspeed = speed * 2;
            float time = 0f;

            while (time < runTime)
            {
                time += Time.deltaTime;
                if (Vector2.Distance(ai.targetTransform.position, ai.position) < 3)
                {
                    ai.maxspeed = speed;
                    ai.canMove = false; ///
                    state.isFacing = true;
                    Debug.Log("PAtk1");
                    ani.ChangeAnimationAttack("PreAtk1");
                    await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                    //await UniTask.WaitForSeconds(1.5f, cancellationToken: token);
                    //ai.canMove = false;
                    Dash();
                    state.isFacing = false;
                    Debug.Log("Atk1");
                    ani.ChangeAnimationAttack("Atk1");
                    await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                    //await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                    //state.rb.velocity = Vector2.zero;
                    await UniTask.DelayFrame(1, cancellationToken: token);
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    ai.canMove = false; ///
                    //state.isFacing = false;
                    ani.ChangeAnimationAttack("PreAtk2");
                    await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                    //await UniTask.WaitForSeconds(1.5f, cancellationToken: token);
                    //ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationAttack("Atk2");
                    await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                    //await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                    //state.rb.velocity = Vector2.zero;
                    await UniTask.DelayFrame(1, cancellationToken: token);
                    ai.canMove = true;


                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);


                    ai.canMove = false;///
                    //state.isFacing = false;
                    ani.ChangeAnimationAttack("PreAtk3");
                    await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                    //await UniTask.WaitForSeconds(1.5f, cancellationToken: token);
                    //ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationAttack("Atk3");
                    await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                    //await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                    //state.rb.velocity = Vector2.zero;
                    await UniTask.DelayFrame(1, cancellationToken: token);
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    if (!overdrive)
                    {
                        ai.canMove = false;///
                        //state.isFacing = false;
                        ani.ChangeAnimationAttack("PreAtk4");
                        await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                        //await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                        //ai.canMove = false;
                        ani.ChangeAnimationAttack("Atk4");
                        await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                        state.ShootBladeslash();
                        await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                        //await UniTask.WaitForSeconds(3f, cancellationToken: token);
                        //state.rb.velocity = Vector2.zero;
                        await UniTask.DelayFrame(1, cancellationToken: token);
                        ai.canMove = true;
                    }
                    else
                    {
                        ai.canMove = false;///
                        //state.isFacing = false;
                        ani.ChangeAnimationAttack("PreAtk4");
                        await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                        //await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                        //ai.canMove = false;
                        ani.ChangeAnimationAttack("Atk4O");
                        await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                        state.ShootBladeslash();
                        await UniTask.WaitForSeconds(1f, cancellationToken: token);
                        state.ShootMissile(token).Forget();
                        await UniTask.WaitForSeconds(1f, cancellationToken: token);
                        LaserFollowIn().Forget();
                        await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                        //await UniTask.WaitForSeconds(4.2f, cancellationToken: token);
                    }
                    

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    break;
                }
                token.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }
            ai.canMove = false;

            ai.maxspeed = speed;
            ani.ChangeAnimationAttack("Wait");
            await UniTask.WaitForSeconds(3, cancellationToken: token);
            ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }
        catch (OperationCanceledException)
        {
            return;
        }        
    } 
     
    //public override void ChangState(BaseState Nextstate)
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

        await UniTask.WhenAll(state.ShootLaserFollowIn(1f, 3f, 1, 4.5f), state.RangeFollow(2f));
        state.DelLaserGun();
    }

    public void Dash()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        dash = true;
        state.rollSpeed = state.dodgeMaxSpeed;
        state.isFacing = false;
    }

    public void DashStart()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        Vector2 dir = (ai.targetTransform.position - ai.position).normalized;

        RaycastHit2D[] raycast = Physics2D.RaycastAll(ai.position, dir, state.dodgeStopRange, LayerMask.GetMask("Obstacle"));
        if (raycast.Length > 0)
        {
            state.rollSpeed = state.dodgeMinimium;
            dash = false;
            state.isFacing = true;
            return;
        }
        state.rollSpeed -= state.rollSpeed * state.dodgeSpeedDropMultiplier * Time.deltaTime;
        if (state.rollSpeed < state.dodgeMinimium)
        {
            dash = false;
            state.rb.velocity = Vector3.zero;
            state.isFacing = true;
        }

        state.rb.velocity = dir * state.rollSpeed;
    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        state.DelLaserGun();
        cancellationToken?.Cancel();
        state.animator.SetBool("NormalAB1FSM", false);
    }
}
