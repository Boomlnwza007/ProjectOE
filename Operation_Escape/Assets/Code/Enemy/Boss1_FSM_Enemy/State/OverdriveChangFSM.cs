using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class OverdriveChangFSM : BaseState
{    
    public OverdriveChangFSM(FSMBoss1EnemySM stateEnemy) : base("OverdriveChang", stateEnemy) { }
    public IAiAvoid ai;
    public float distance;
    public float speed;
    public float time;

    public override void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;

        state.animator.SetBool("OverdriveChangFSM", true);
        ai = state.ai;
        ai.canMove = false;
        time = 0;
        Debug.Log("Start");
        var ani = state.boss1AniControl;
        ani.ChangeAnimationAttack("ChangeState");
        Over().Forget();
    }

    public async UniTask Over()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        var ani = state.boss1AniControl;
        ani.ChangeAnimationAttack("ChangeState");
        await UniTask.WaitUntil(() => ani.endAnim);
        state.JumpCenter();
        Debug.Log("Comple");
        state.overdrive = true;
        state.overdriveChang = false;
        stateMachine.ChangState(state.checkDistanceState);
    }

    public override void UpdateLogic()
    {
        //time += Time.deltaTime;
        //if (time > 3)
        //{
        //    ((FSMBoss1EnemySM)stateMachine).JumpCenter();
        //    Debug.Log("Comple");
        //    ((FSMBoss1EnemySM)stateMachine).overdrive = true;
        //    ((FSMBoss1EnemySM)stateMachine).overdriveChang = false;
        //    stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        //}
    }

    public override void Exit()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        //cancellationToken?.Cancel();
        state.animator.SetBool("OverdriveChangFSM", false);
    }
}
