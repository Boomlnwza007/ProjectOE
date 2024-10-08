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
        var state = (FSMBoss1EnemySM)stateMachine;
        state.animator.SetBool("DashAB1FSM", true);
        ai = state.ai;
        overdrive = state.overdrive;
        speed = ai.maxspeed;
        
        ai.canMove = true;
        if (!overdrive)
        {
            if (!state.rangeAState.rangeAttack)
            {                
                await UniTask.WaitForSeconds(1f);
                state.animator.SetBool("Attacking", false);
                state.animator.SetTrigger("DashExit");                
                ChangState(state.normalAState);
                return;
            }
            else
            {
                state.rangeAState.rangeAttack = false;
                Changemode = 1;
                Charg = 1;
                Attack().Forget();
            }
        }
        else
        {
            Changemode = 0.5f;
            Charg = 0.3f;
            Attack().Forget();
        }

        
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

    public async UniTaskVoid Attack()
    {
        ai.canMove = false;
        var state = ((FSMBoss1EnemySM)stateMachine);        
        Debug.Log("Change Mode 1s");
        await UniTask.WaitForSeconds(Changemode);
        Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.StartDash);
        Debug.Log("Charg 1s");
        await UniTask.WaitForSeconds(Charg);
        ai.canMove = true;
        state.animator.SetBool("Attacking", true);
        if (overdrive)
        {
            ai.maxspeed = speed * 3;
        }
        else
        {
            ai.maxspeed = speed * 2;
        }
        float time = 0;
        while (time < 4f)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.targetTransform.position, ai.position) < 3)
            {
                Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.StopDash);
                ai.maxspeed = speed;
                if (overdrive)
                {
                    Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.AfterDash);
                    await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.3f, 0), AimMelee(0.4f)); // AfDash
                    Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.Atk4);
                    await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.3f, 1), AimMelee(0.4f)); // hit 4
                    Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.Wait);
                    await UniTask.WaitForSeconds(1);
                    Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.RangeAtk);
                    await LaserFollowIn();
                    Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.Wait);
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
                    Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.AfterDash);
                    await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.7f, 0), AimMelee(0.4f)); // AfDash
                    Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.Wait);
                    ai.canMove = false;
                    await UniTask.WaitForSeconds(1);
                    ai.canMove = true;
                    ChangState(state.normalAState);
                }
                state.animator.SetBool("Attacking", false);
                return;
            }

            await UniTask.Yield();
        }
        ai.maxspeed = speed;
        Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.StopDash);
        await UniTask.WaitForSeconds(0.2f);
        Boss1AniControl.boss1AniControl.ChangeAnimationState(Boss1AniControl.StateBoss.Wait);
        state.animator.SetBool("Attacking", false);
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

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        state.animator.SetBool("DashAB1FSM", false);
    }
}
