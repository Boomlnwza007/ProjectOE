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
        var state = (FSMBoss1EnemySM)stateMachine;
        state.animator.SetBool("NormalAB1FSM", true);
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        overdrive = ((FSMBoss1EnemySM)stateMachine).overdrive;
        ai.canMove = true;
        speed = ai.maxspeed;
        if (!overdrive)
        {
            Charg = 0.6f;
            runTime = 4f;
        }
        else
        {
            Charg = 0.4f;
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

    public async UniTaskVoid Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        ai.maxspeed = speed * 2;
        float time = 0f;
        while (time < runTime)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.targetTransform.position, ai.position) < 3)
            {
                var animComtrol = Boss1AniControl.boss1AniControl;
                ai.maxspeed = speed;
                state.animator.SetBool("Attacking", true);
                animComtrol.ChangeAnimationState(Boss1AniControl.StateBoss.PreAtk1_2);
                await UniTask.WhenAll(state.MeleeHitzone(1f,0.5f, 2), AimMelee(0.8f),AttackAnimation(1f,0.5f,Boss1AniControl.StateBoss.Atk1_2)); // hit 1
                await UniTask.WhenAll(state.MeleeHitzone(0.1f, 0.5f, 2), AimMelee(0.8f)); // hit 2

                animComtrol.ChangeAnimationState(Boss1AniControl.StateBoss.PreAtk3);
                await UniTask.WhenAll(state.MeleeHitzone(Charg, 0.5f, 3), AimMelee(0.8f), AttackAnimation(Charg, 0.5f, Boss1AniControl.StateBoss.Atk3)); // hit 3
                animComtrol.ChangeAnimationState(Boss1AniControl.StateBoss.Atk4);
                await UniTask.WhenAll(state.MeleeHitzone(0.2f, 0.5f, 1), AimMelee(0.8f)); // hit 4
                if (overdrive)
                {
                    await UniTask.WhenAll(state.MeleeHitzone(0.5f, 0.3f, 1), AimMelee(0.4f)); // hit 4
                    animComtrol.ChangeAnimationState(Boss1AniControl.StateBoss.Wait);
                    await UniTask.WaitForSeconds(1);
                    animComtrol.ChangeAnimationState(Boss1AniControl.StateBoss.RangeAtk);
                    await LaserFollowIn();
                    animComtrol.ChangeAnimationState(Boss1AniControl.StateBoss.Wait);
                    state.DelLaserGun();
                }
                animComtrol.ChangeAnimationState(Boss1AniControl.StateBoss.Wait);

                state.animator.SetBool("Attacking", false);
                ai.canMove = false;
                await UniTask.WaitForSeconds(1);
                ai.canMove = true;
                ChangState(state.checkDistanceState);
                return;
            }
            await UniTask.Yield();
        }

        ai.maxspeed = speed;
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
        state.animator.SetBool("NormalAB1FSM", false);
    }

    public async UniTask AttackAnimation(float charge, float duration, Boss1AniControl.StateBoss stateBoss)
    {
        var animComtrol = Boss1AniControl.boss1AniControl;
        await UniTask.WaitForSeconds(charge);
        animComtrol.ChangeAnimationState(stateBoss);
    }

    public async UniTask AttackAnimation(float charge, float duration, Boss1AniControl.StateBoss PrestateBoss, Boss1AniControl.StateBoss stateBoss)
    {
        var animComtrol = Boss1AniControl.boss1AniControl;
        animComtrol.ChangeAnimationState(PrestateBoss);
        await UniTask.WaitForSeconds(charge);
        animComtrol.ChangeAnimationState(stateBoss);
        await UniTask.WaitForSeconds(duration);
    }
}
