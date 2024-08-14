using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NormalAB1FSM : BaseState
{
    public NormalAB1FSM(FSMBoss1EnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;
    bool followAim;
    bool followMe;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;
        if (followAim)
        {
            ((FSMBoss1EnemySM)stateMachine).LaserFollow();
        }

        if (followMe)
        {
            ((FSMBoss1EnemySM)stateMachine).MeleeFollow();
        }
    }

    public async UniTask Attack()
    {
        ai.Maxspeed = speed * 2;
        await UniTask.WaitForSeconds(4f);
        ai.Maxspeed = speed;
        if (Vector2.Distance(ai.target.position, ai.position) < 3)
        {
            await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).MeleeHitzone(1f, 1), AimMelee(0.8f));
            await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).MeleeHitzone(0.3f, 2), AimMelee(0.4f));
            await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).MeleeHitzone(1f, 0), AimMelee(0.8f));
            ai.canMove = false;
            await UniTask.WaitForSeconds(2);
            ai.canMove = true;
            stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).normalAState);
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).ShootLaser(0.4f+(0.1f)*6f, 0.3f), Aim(0.3f + (0.1f) * 6f));
            }
            stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }

    }

    public async UniTask Aim(float wait)
    {
        followAim = true;
        await UniTask.WaitForSeconds(wait);
        followAim = false;
    }

    public async UniTask AimMelee(float wait)
    {
        followAim = true;
        await UniTask.WaitForSeconds(wait);
        followAim = false;
    }
}
