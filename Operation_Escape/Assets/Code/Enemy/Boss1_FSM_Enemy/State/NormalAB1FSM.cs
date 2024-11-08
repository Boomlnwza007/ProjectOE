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
    bool followMe;

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
            Attack().Forget();

        }
        else
        {
            runTime = 3f;
            AttackO().Forget();
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;

        if (followMe)
        {
            ((FSMBoss1EnemySM)stateMachine).MeleeFollow();  
        }
    }

    public async UniTaskVoid Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = Boss1AniControl.boss1AniControl;
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
                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk1");
                    await UniTask.DelayFrame(110, cancellationToken: token);
                    ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationState("Atk1");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.rb.velocity = Vector2.zero;
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk2");
                    await UniTask.DelayFrame(110, cancellationToken: token);
                    ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationState("Atk2");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.rb.velocity = Vector2.zero;
                    ai.canMove = true;


                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);


                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk3");
                    await UniTask.DelayFrame(110, cancellationToken: token);
                    ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationState("Atk3");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.rb.velocity = Vector2.zero;
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk4");
                    await UniTask.DelayFrame(50, cancellationToken: token);
                    ai.canMove = false;
                    ani.ChangeAnimationState("Atk4");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.ShootBladeslash();
                    await UniTask.DelayFrame(190, cancellationToken: token);
                    state.rb.velocity = Vector2.zero;
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    break;
                }
                token.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }
            ai.canMove = false;

            ai.maxspeed = speed;
            ani.ChangeAnimationState("Wait");
            await UniTask.WaitForSeconds(3, cancellationToken: token);
            ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }
        catch (OperationCanceledException)
        {
            return;
        }        
    }

    public async UniTaskVoid AttackO()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = Boss1AniControl.boss1AniControl;
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
                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk1");
                    await UniTask.DelayFrame(110, cancellationToken: token);
                    ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationState("Atk1");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.rb.velocity = Vector2.zero;
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk2");
                    await UniTask.DelayFrame(110, cancellationToken: token);
                    ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationState("Atk2");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.rb.velocity = Vector2.zero;
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk3");
                    await UniTask.DelayFrame(110, cancellationToken: token);
                    ai.canMove = false;
                    Dash();
                    ani.ChangeAnimationState("Atk3");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.rb.velocity = Vector2.zero;
                    ai.canMove = true;

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    ai.canMove = true;
                    state.isFacing = false;
                    ani.ChangeAnimationState("PreAtk4");
                    await UniTask.DelayFrame(50, cancellationToken: token);
                    ai.canMove = false;
                    ani.ChangeAnimationState("Atk4O");
                    await UniTask.DelayFrame(10, cancellationToken: token);
                    state.ShootBladeslash();
                    await UniTask.DelayFrame(60, cancellationToken: token);
                    state.ShootMissile().Forget();
                    await UniTask.DelayFrame(60, cancellationToken: token);
                    LaserFollowIn().Forget();
                    await UniTask.DelayFrame(60, cancellationToken: token);

                    state.isFacing = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);

                    ai.canMove = true;
                    break;
                }

                token.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }

            ani.ChangeAnimationState("Wait");
            ai.maxspeed = speed;
            await UniTask.WaitForSeconds(3, cancellationToken: token);
            ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }
        catch (OperationCanceledException)
        {
            return;
        }       
    }

    public async UniTask AimMelee(float wait)
    {
        followMe= true;
        await UniTask.WaitForSeconds(wait);
        followMe = false;
    }

    public void ChangState(BaseState Nextstate)
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        if (!state.attacking)
        {
            state.JumpCenter();
            state.ChangState(state.idleState);
            return;

        }

        if (state.overdriveChang)
        {
            state.ChangState(state.overdriveChangState);
        }
        else
        {
            state.ChangState(Nextstate);
        }
    }

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
        Vector2 dir = Vector2.zero;
        if (state.isFacingRight)
        {
            dir = Vector2.right;
        }
        else
        {
            dir = Vector2.left;
        }
        state.rb.AddForce(dir * state.forcePush, ForceMode2D.Impulse);

    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        state.DelLaserGun();
        cancellationToken?.Cancel();
        state.animator.SetBool("NormalAB1FSM", false);
    }
}
