using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class RangeAB1Fsm : BaseState
{
    public RangeAB1Fsm(FSMBoss1EnemySM stateMachine) : base("RangeAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public bool follow;
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
        ai.destination = ai.target.position;
        if (follow)
        {
            ((FSMBoss1EnemySM)stateMachine).LaserFollow();
        }
    }

    public async UniTask Attack()
    {
        Debug.Log(ai.canMove + " " + ((FSMBoss1EnemySM)stateMachine).ai.canMove);
        ai.canMove = false;
        await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(charge, 0.2f), Aim(0.8f));
        CheckDistance();
        await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(charge, 0.2f), Aim(0.8f));
        CheckDistance();
        await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(charge, 0.2f), Aim(0.8f));
        if (overdrive)
        {
            await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(charge, 4f), Aim(charge+4f));
        }

        UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.4f + (0.1f), 0.3f), Aim(0.3f + (0.1f))).Forget();
        UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.4f + (0.2f), 0.3f), Aim(0.3f + (0.2f))).Forget();
        UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.4f + (0.3f), 0.3f), Aim(0.3f + (0.3f))).Forget();
        UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.4f + (0.4f), 0.3f), Aim(0.3f + (0.4f))).Forget();
        UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.4f + (0.5f), 0.3f), Aim(0.3f + (0.5f))).Forget();
        await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.4f + (0.6f), 0.3f), Aim(0.3f + (0.6f)));

        ai.canMove = false;
        await UniTask.WaitForSeconds(0.5f);
        ai.canMove = true;
        stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).dashAState);
    }

    public void CheckDistance()
    {
        if (!overdrive)
        {
            if (Vector2.Distance(ai.target.position, ai.position) < 3)
            {
                stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).dashAState);
            }
        }        
    }

    public async UniTask Aim(float wait)
    {
        follow = true;
        await UniTask.WaitForSeconds(wait);
        follow = false;
    }
}
