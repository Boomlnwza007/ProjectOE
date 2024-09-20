using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class RangeAB1Fsm : BaseState
{
    public RangeAB1Fsm(FSMBoss1EnemySM stateMachine) : base("RangeAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public bool overdrive;
    public bool rangeAttack;
    float charge;
    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        overdrive = ((FSMBoss1EnemySM)stateMachine).overdrive;
        speed = ai.Maxspeed;
        ai.canMove = false;
        rangeAttack = true;
        charge = 1;
        if (overdrive)
        {
            charge = 0.3f;
        }
        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;
    }

    public async UniTask Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        state.CreatLaserGun();
        ai.canMove = false;
        await state.ShootLaser(charge, 0.5f, 1, charge - 0.1f);        
        if (CheckDistance())
        {
            return;
        }
        await state.ShootLaser(charge, 0.5f, 1, charge - 0.1f);
        if (CheckDistance())
        {
            return;
        }
        await state.ShootLaser(charge, 0.5f, 1, charge - 0.1f);
        if (overdrive)
        {
            await UniTask.WhenAll(state.ShootLaser(charge, 2f, 1, charge + 2f,3.5f), Missil());
        }
        state.DelLaserGun();
        await state.ShootMissile();

        ai.canMove = false;
        await UniTask.WaitForSeconds(0.5f);
        ai.canMove = true;

        ChangState(((FSMBoss1EnemySM)stateMachine).dashAState);
    }

    public async UniTask Missil()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        await UniTask.WaitForSeconds(0.5f);
        await state.ShootMissile();
        await UniTask.WaitForSeconds(1f);
        state.ShootMissile().Forget();
        await UniTask.WaitForSeconds(0.5f);
        state.ShootMissile().Forget();
        await UniTask.WaitForSeconds(1);
    }

    public bool CheckDistance()
    {
        bool distance = false;
        if (!overdrive)
        {
            if (Vector2.Distance(ai.targetTransform.position, ai.position) < 3)
            {
                ((FSMBoss1EnemySM)stateMachine).DelLaserGun();
                ChangState(((FSMBoss1EnemySM)stateMachine).dashAState);
                distance = true;
            }
        }
        return distance;
    }

    public void ChangState(BaseState Nextstate)
    {
        var state = (FSMBoss1EnemySM)stateMachine;
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
}
