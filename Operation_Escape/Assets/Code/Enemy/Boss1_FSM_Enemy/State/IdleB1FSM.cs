using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleB1FSM : BaseState
{
    public IdleB1FSM(FSMBoss1EnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 15f;
    private bool ready;

    public override void Enter()
    {
        ((FSMBoss1EnemySM)stateMachine).animator.SetBool("IdleB1FSM", true);
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        ai.destination = ai.position;
        ai.canMove = false;
        ready = false;
    }

    public override void UpdateLogic()
    {
        if (Vector2.Distance(ai.position, ai.targetTransform.position) < ((FSMBoss1EnemySM)stateMachine).visRange && ((FSMBoss1EnemySM)stateMachine).attacking && !ready)
        {
            ready = true;
            Reay().Forget();
        }
    }

    public override void Exit()
    {
        ((FSMBoss1EnemySM)stateMachine).animator.SetBool("IdleB1FSM", false);
    }

    public async UniTask Reay()
    {
        var ani = ((FSMBoss1EnemySM)stateMachine).boss1AniControl;
        ani.ChangeAnimationAttack("ChangeState");
        await UniTask.WaitUntil(() => ani.endAnim);
        ani.ChangeAnimationAttack("Wait");
        ai.canMove = true;
        stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
    }
}
