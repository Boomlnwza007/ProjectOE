using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class M1IdleFSM : BaseState
{
    public M1IdleFSM(FSMMinion1EnemySM stateEnemy) : base("IdleState", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    public IAiAvoid ai;
    private bool ready;

    public override void Enter()
    {
        ai = ((FSMMinion1EnemySM)stateMachine).ai;
        ai.canMove = false;
        ready = false;
    }

    public override void UpdateLogic()
    {
        var state = (FSMMinion1EnemySM)stateMachine;
        if (Vector2.Distance(ai.position, ai.targetTransform.position) < state.visRange && state.attacking && !ready)
        {
            ready = true;
            Reay().Forget();
        }
    }

    public override void Exit()
    {
        cancellationToken.Cancel();
    }

    public async UniTask Reay()
    {
        var state = (FSMMinion1EnemySM)stateMachine;
        await UniTask.WaitForSeconds(1);
        ChangState(state.checkDistance);
    }
}
