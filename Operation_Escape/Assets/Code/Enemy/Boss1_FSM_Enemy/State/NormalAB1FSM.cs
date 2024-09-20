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
    public float runTime;
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
            Charg = 0.7f;
            runTime = 4f;
        }
        else
        {
            Charg = 0.5f;
            runTime = 2f;
        }
        Attack().Forget();
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

    public async UniTask Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        ai.Maxspeed = speed * 2;
        float time = 0f;
        while (time < runTime)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.targetTransform.position, ai.position) < 3)
            {
                ai.Maxspeed = speed;
                await UniTask.WhenAll(state.MeleeHitzone(1f,0.5f, 2), AimMelee(0.8f));
                await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.5f, 2), AimMelee(0.8f));
                await UniTask.WhenAll(state.MeleeHitzone(Charg, 0.5f, 3), AimMelee(0.8f));
                await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.5f, 1), AimMelee(0.8f));
                if (overdrive)
                {
                    await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.3f, 1), AimMelee(0.4f));
                    await UniTask.WaitForSeconds(1);
                    await LaserFollowIn();
                    state.DelLaserGun();
                }

                ai.canMove = false;
                await UniTask.WaitForSeconds(1);
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

    public async UniTask LaserFollowIn()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        //Vector2 directionToPlayer = (ai.targetTarnsform.position - ai.position).normalized;
        //float angleDirectionToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        //float angleLeft = angleDirectionToPlayer + 90;
        //float angleRight = angleDirectionToPlayer - 90;
        //float[] angle = { angleLeft, angleRight };

        //state.CreatLaserGun(angle);

        await UniTask.WhenAll(state.ShootLaserFollowIn(2f, 3f, 1, 4.5f), state.RangeFollow(2f));

    }
}
