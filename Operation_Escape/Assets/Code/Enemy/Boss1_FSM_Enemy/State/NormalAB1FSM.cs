using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NormalAB1FSM : BaseState
{
    public NormalAB1FSM(FSMBoss1EnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    bool followAim;
    bool followMe;


    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        ai.canMove = true;
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
        float time = 0f;
        while (time < 4f)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.target.position, ai.position) < 3)
            {
                await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).MeleeHitzone(1f, 1), AimMelee(0.8f));
                await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).MeleeHitzone(0.5f, 2), AimMelee(0.4f));
                await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).MeleeHitzone(1f, 0), AimMelee(0.8f));
                ai.canMove = false;
                await UniTask.WaitForSeconds(2);
                ai.canMove = true;
                ai.Maxspeed = speed;
                ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
                return;
            }
            await UniTask.Yield();
        }

        await ((FSMBoss1EnemySM)stateMachine).ShootMissile();

        ai.Maxspeed = speed;
        ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
    }

    public async UniTask Aim(float wait)
    {
        followAim = true;
        await UniTask.WaitForSeconds(wait);
        followAim = false;
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
        if (state.overdriveChang)
        {
            state.ChangState(state.overdriveChangState);
        }
        else
        {
            state.ChangState(Nextstate);
        }
    }


}
