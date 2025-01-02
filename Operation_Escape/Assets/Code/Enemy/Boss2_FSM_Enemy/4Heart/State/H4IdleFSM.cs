using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H4IdleFSM : BaseState
{
    public H4IdleFSM(FSMHeart4EnemySM stateEnemy) : base("IdleState", stateEnemy) { }
    public IAiAvoid ai;
    private bool ready;

    public override void Enter()
    {
        ai = ((FSMHeart4EnemySM)stateMachine).ai;
        ai.canMove = false;
        ready = false;
    }

    public override void UpdateLogic()
    {
        var state = (FSMHeart4EnemySM)stateMachine;
        if (Vector2.Distance(ai.position, ai.targetTransform.position) < state.visRange && state.attacking && !ready)
        {
            ready = true;
            Reay().Forget();
        }
    }

    public async UniTask Reay()
    {
        var state = (FSMHeart4EnemySM)stateMachine;
        await UniTask.WaitForSeconds(1);
        state.Shield.ShieldIsOn(true);
        ChangState(state.attack);
    }
}
