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
    public bool pull;

    // Start is called before the first frame update
    public override async void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        ai = state.ai;
        overdrive = state.overdrive;
        speed = ai.maxspeed;
        pull = false;
        ai.canMove = true;
        if (!overdrive)
        {
            if (!state.rangeAState.rangeAttack)
            {                
                await UniTask.WaitForSeconds(1f);             
                ChangState(state.normalAState);
                return;
            }
            else
            {
                state.rangeAState.rangeAttack = false;
                Changemode = 1;
                Charg = 1;
                AttackN().Forget();
            }
        }
        else
        {
            Changemode = 0.5f;
            Charg = 0.3f;
            AttackO().Forget();
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

        if (pull)
        {
            //PullPlayer();
        }
    }

    public async UniTaskVoid AttackN()
    {
        ai.canMove = false;
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = Boss1AniControl.boss1AniControl;
        state.isFacing = false;
        ani.ChangeAnimationState("StartJump");
        await UniTask.WaitForSeconds(2.1f);
        ai.canMove = true;
        ani.ChangeAnimationState("AirJump");
        await UniTask.WaitForSeconds(3f);
        ai.canMove = false;
        ani.ChangeAnimationState("StopJump");
        await UniTask.WaitForSeconds(0.15f);
        state.isFacing = true;
        ani.ChangeAnimationState("Wait");        
        await UniTask.WaitForSeconds(2f);
        state.isFacing = false;
        ani.ChangeAnimationState("AfterDash");
        await UniTask.WaitForSeconds(5.1f);
        state.isFacing = true;
        ani.ChangeAnimationState("Wait");
        await UniTask.WaitForSeconds(3f);
        ai.canMove = true;
        ChangState(state.normalAState);
    }

    public async UniTaskVoid AttackO()
    {
        ai.canMove = false;
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = Boss1AniControl.boss1AniControl;
        ani.ChangeAnimationState("StartJump");
        await UniTask.WaitForSeconds(2.1f);
        ai.canMove = true;
        ani.ChangeAnimationState("AirJumpO");
        await UniTask.WaitForSeconds(3f);
        ai.canMove = false;
        state.isFacing = false;
        ani.ChangeAnimationState("StopJump");
        await UniTask.WaitForSeconds(0.15f);
        state.isFacing = true;

        ani.ChangeAnimationState("Wait");
        await UniTask.WaitForSeconds(2f);
        PullPlayer().Forget();

        state.isFacing = false;
        ani.ChangeAnimationState("AfterDash");
        await UniTask.WaitForSeconds(6.2f);
        state.isFacing = true;

        ani.ChangeAnimationState("Wait");
        await UniTask.WaitForSeconds(3f);
        ai.canMove = true;
        if (Random.value > 0.5f)
        {
            ChangState(state.normalAState);
        }
        else
        {
            ChangState(state.rangeAState);
        }
    }

    public async UniTask PullPlayer()
    {
        await UniTask.WaitForSeconds(3);
        float distance = Vector2.Distance(ai.position, ai.targetTransform.position);
        Vector2 dir = (ai.position - ai.targetTransform.position).normalized;
        float range = 30f; // ระยะที่เริ่มดึงดูด
        float pullStrength = 10f; // กำหนดแรงดึง
        if (distance <= range)
        {
            PlayerControl.control.playerMovement.rb.AddForce(dir * pullStrength / distance, ForceMode2D.Force);
        }
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
