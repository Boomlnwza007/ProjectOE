using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DashAB1FSM : BaseState
{
    public DashAB1FSM(FSMBoss1EnemySM stateMachine) : base("DashAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public float Changemode;
    public float Charg;
    bool followMe;
    public bool overdrive;

    // Start is called before the first frame update
    public override async void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        overdrive = ((FSMBoss1EnemySM)stateMachine).overdrive;
        speed = ai.Maxspeed;
        ai.canMove = true;

        if (!overdrive)
        {
            if (!((FSMBoss1EnemySM)stateMachine).rangeAState.rangeAttack)
            {
                Changemode = 1;
                Charg = 1;
                await UniTask.WaitForSeconds(1f);
                ((FSMBoss1EnemySM)stateMachine).rangeAState.rangeAttack = false;
                ChangState(((FSMBoss1EnemySM)stateMachine).normalAState);
                return;
            }
        }
        else
        {
            Changemode = 0.5f;
            Charg = 0.3f;
        }

        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTarnsform.position;

        if (followMe)
        {
            ((FSMBoss1EnemySM)stateMachine).MeleeFollow();
        }


    }

    public async UniTask Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        Debug.Log("Change Mode 1s");
        await UniTask.WaitForSeconds(Changemode);
        Debug.Log("Charg 1s");
        await UniTask.WaitForSeconds(Charg);
        if (overdrive)
        {
            ai.Maxspeed = speed * 3;
        }
        else
        {
            ai.Maxspeed = speed * 2;
        }

        float time = 0;
        while (time < 4f)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.targetTarnsform.position, ai.position) < 3)
            {
                ai.Maxspeed = speed;
                if (overdrive)
                {
                    await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.3f, 0), AimMelee(0.4f));
                    state.CreatLaserGun(Set());
                    await UniTask.WhenAll( state.ShootLaserFollow(0.5f, 3f, 1, 4.5f),state.MeleeHitzone(0.5f, 0.3f, 1), AimMelee(0.4f));
                    state.DelLaserGun();
                    await UniTask.WaitForSeconds(1);
                    if (Random.value < 0.5f)
                    {
                        ChangState(state.normalAState);
                    }
                    else
                    {
                        ChangState(state.rangeAState);
                    }

                }
                else
                {
                    await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.7f, 0), AimMelee(0.4f));
                    ai.canMove = false;
                    await UniTask.WaitForSeconds(1);
                    ai.canMove = true;
                    ChangState(state.normalAState);
                }
                return;
            }
            await UniTask.Yield();
        }
        ai.Maxspeed = speed;
        ChangState(state.normalAState);
    }

    public async UniTask AimMelee(float wait)
    {
        followMe = true;
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

    public float[] Set()
    {
        Vector2 directionToPlayer = (ai.targetTarnsform.position - ai.position).normalized;
        float angleDirectionToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        float angleLeft = angleDirectionToPlayer + 90;
        float angleRight = angleDirectionToPlayer - 90;
        float[] angle = { angleLeft, angleRight };
        return angle;
    }

}
