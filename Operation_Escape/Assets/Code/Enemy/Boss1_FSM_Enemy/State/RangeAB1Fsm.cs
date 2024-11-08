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
            charge = 0.3f;
        }
        else
        {
            charge = 1f;
        }

        Attack().Forget();
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

        try
        {
            state.CreatLaserGun();
            if (overdrive)
            {
                state.Setdamage(60);
            }
            ai.canMove = false;

            Boss1AniControl.boss1AniControl.ChangeAnimationState("RangeAtk");
            await state.ShootLaser(charge, 0.5f, 1, charge - 0.1f); //1
            await state.ShootMissile();
            if (CheckDistance())
            {
                Boss1AniControl.boss1AniControl.ChangeAnimationState("Wait");
                return;
            }
            Boss1AniControl.boss1AniControl.ChangeAnimationState("RangeAtk");
            await state.ShootLaser(charge, 0.5f, 1, charge - 0.1f); //2
            await state.ShootMissile();
            if (CheckDistance())
            {
                Boss1AniControl.boss1AniControl.ChangeAnimationState("Wait");

                return;
            }
            Boss1AniControl.boss1AniControl.ChangeAnimationState("RangeAtk");
            await state.ShootLaser(charge, 0.5f, 1, charge - 0.1f); //3
            await state.ShootMissile();
            if (overdrive)
            {
                Boss1AniControl.boss1AniControl.ChangeAnimationState("RangeAtk");
                await UniTask.WhenAll(state.ShootLaser(charge, 6f, 1, charge + 6f, 3.5f), Missil());
            }
            state.DelLaserGun();
            Boss1AniControl.boss1AniControl.ChangeAnimationState("Wait");

            ai.canMove = false;
            await UniTask.WaitForSeconds(3f);
            ai.canMove = true;

            Boss1AniControl.boss1AniControl.ChangeAnimationState("Wait");
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
        await state.ShootMissile(1);
        await UniTask.WaitForSeconds(2f);
        await state.ShootMissile(1);
        await UniTask.WaitForSeconds(2f);
        await state.ShootMissile(1);
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
                state.animator.SetBool("Attacking", false);
                ChangState(state.dashAState);
                distance = true;
            }
        }
        return distance;
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
