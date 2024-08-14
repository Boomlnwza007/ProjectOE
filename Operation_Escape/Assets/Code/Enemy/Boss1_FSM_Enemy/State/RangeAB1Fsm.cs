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
    float time;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        ai.canMove = false;
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
        for (int i = 0; i < 3; i++)
        {
            await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(1, 0.5f), Aim(0.8f));

        }

        for (int i = 0; i < 6; i++)
        {
            await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.5f, 0.3f), Aim(0.3f));

        }
        ai.canMove = false;
        await UniTask.WaitForSeconds(0.5f);
        ai.canMove = true;
        stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
    }

    public async UniTask Aim(float wait)
    {
        follow = true;
        await UniTask.WaitForSeconds(wait);
        follow = false;
    }
}
