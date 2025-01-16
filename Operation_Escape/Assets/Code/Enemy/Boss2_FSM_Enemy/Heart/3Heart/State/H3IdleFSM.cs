using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H3IdleFSM : BaseState
{
    public H3IdleFSM(FSMHeart3EnemySM stateEnemy) : base("IdleState", stateEnemy) { }
    public IAiAvoid ai;
    private bool ready;

    public override void Enter()
    {
        ai = ((FSMHeart3EnemySM)stateMachine).ai;
        ai.canMove = false;
        ready = false;
    }

    public override void UpdateLogic()
    {
        var state = (FSMHeart3EnemySM)stateMachine;
        if (Vector2.Distance(ai.position, ai.targetTransform.position) < state.visRange && state.attacking && !ready)
        {
            ready = true;
            Reay().Forget();
        }
    }

    public async UniTask Reay()
    {
        var state = (FSMHeart3EnemySM)stateMachine;
        await UniTask.WaitForSeconds(1);
        state.shield.ShieldIsOn(true);
        ChangState(state.attack);
    }
}
