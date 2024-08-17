using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NormalAB1FSM : BaseState
{
    public NormalAB1FSM(FSMBoss1EnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public float Charg;
    public bool overdrive;
    bool followMe;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        overdrive = ((FSMBoss1EnemySM)stateMachine).overdrive;
        ai.canMove = true;
        speed = ai.Maxspeed;
        if (!overdrive)
        {
            Charg = 1f;
        }
        else
        {
            Charg = 0.5f;
        }
        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;

        if (followMe)
        {
            ((FSMBoss1EnemySM)stateMachine).MeleeFollow();  
        }
    }

    public async UniTask Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        ai.Maxspeed = speed * 2;
        float time = 0f;
        while (time < 4f)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.target.position, ai.position) < 3)
            {
                ai.Maxspeed = speed;
                await UniTask.WhenAll(state.MeleeHitzone(1f,0.5f, 2), AimMelee(0.8f));
                await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.5f, 2), AimMelee(0.8f));
                await UniTask.WhenAll(state.MeleeHitzone(Charg, 0.5f, 4), AimMelee(0.8f));
                await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.5f, 1), AimMelee(0.8f));
                if (overdrive)
                {
                    await UniTask.WhenAll(state.ShootLaserFollow(0.5f, 3f, 1, 4.5f), state.MeleeHitzone(0.5f, 0.3f, 1), AimMelee(0.4f));
                }

                ai.canMove = false;
                await UniTask.WaitForSeconds(2);
                ai.canMove = true;
                ChangState(state.checkDistanceState);
                return;
            }
            await UniTask.Yield();
        }

        ai.Maxspeed = speed;
        ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
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
