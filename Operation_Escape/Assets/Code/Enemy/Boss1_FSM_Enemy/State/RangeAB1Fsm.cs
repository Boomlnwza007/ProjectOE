using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class RangeAB1Fsm : BaseState
{
    public RangeAB1Fsm(FSMBoss1EnemySM stateMachine) : base("RangeAttack", stateMachine) { }
    private CancellationTokenSource cancellationToken;
    public IAiAvoid ai;
    public float speed;
    public bool overdrive;
    public bool rangeAttack;
    float charge;
    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        cancellationToken = new CancellationTokenSource();
        state.animator.SetBool("RangeAB1Fsm", true);
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        overdrive = ((FSMBoss1EnemySM)stateMachine).overdrive;
        speed = ai.maxspeed;
        ai.canMove = false;
        rangeAttack = true;

        if (overdrive)
        {
            charge = 0.5f;
        }
        else
        {
            charge = 1f;
        }

        Attack().Forget();
        state.normalAState.countATK = 0;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;
    }

    public async UniTaskVoid Attack()
    {
        var token = cancellationToken.Token;
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = state.boss1AniControl;

        try
        {
            state.CreatLaserGun();
            if (overdrive)
            {
                state.Setdamage(60);
            }
            ai.canMove = false;

            ani.ChangeAnimationAttack("RangeAtk");
            await UniTask.WhenAll(state.ShootLaser(charge, 0.5f, 1, charge - 0.1f), state.ShootMissile(token));
            if (CheckDistance())
            {
                //ChangState(((FSMBoss1EnemySM)stateMachine).normalAState);
                ani.ChangeAnimationAttack("Wait");
                return;
            }
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);

            ani.ChangeAnimationAttack("RangeAtk");
            await UniTask.WhenAll(state.ShootLaser(charge, 0.5f, 1, charge - 0.1f), state.ShootMissile(token));

            if (CheckDistance())
            {
                //ChangState(((FSMBoss1EnemySM)stateMachine).normalAState);
                ani.ChangeAnimationAttack("Wait");
                return;
            }
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);

            ani.ChangeAnimationAttack("RangeAtk");
            await UniTask.WhenAll(state.ShootLaser(charge, 0.5f, 1, charge - 0.1f), state.ShootMissile(token));

            if (overdrive)
            {
                await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
                ani.ChangeAnimationAttack("RangeAtk");
                await UniTask.WhenAll(state.ShootLaser(charge, 6f, 1, charge + 6f, 3.5f), Missil());
            }
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            state.DelLaserGun();
            ani.ChangeAnimationAttack("Wait");

            ai.canMove = false;
            ani.ChangeAnimationAttack("BossStop");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.animator.SetTrigger("endStop");
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            ai.canMove = true;

            ani.ChangeAnimationAttack("Wait");
            ChangState(((FSMBoss1EnemySM)stateMachine).dashAState);
        }
        catch (OperationCanceledException)
        {
            return;
        }               
    }

    public async UniTask Missil()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        var token = cancellationToken.Token;
        await state.ShootMissile(1, token);
        await UniTask.WaitForSeconds(2f);
        await state.ShootMissile(1, token);
        await UniTask.WaitForSeconds(2f);
        await state.ShootMissile(1, token);
    }

    public bool CheckDistance()
    {
        bool distance = false;
        if (!overdrive)
        {
            if (Vector2.Distance(ai.targetTransform.position, ai.position) < 3)
            {
                var state = ((FSMBoss1EnemySM)stateMachine);
                state.DelLaserGun();
                ChangState(state.normalAState);
                distance = true;
            }
        }
        return distance;
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

    public float[] Set()
    {
        Vector2 directionToPlayer = (ai.targetTransform.position - ai.position).normalized;
        float angleDirectionToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        float angleLeft = angleDirectionToPlayer + 90;
        float angleRight = angleDirectionToPlayer - 90;
        float[] angle = { angleLeft, angleRight };
        return angle;
    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        cancellationToken?.Cancel();
        state.DelLaserGun();
        state.animator.SetBool("RangeAB1Fsm", false);
    }
}
