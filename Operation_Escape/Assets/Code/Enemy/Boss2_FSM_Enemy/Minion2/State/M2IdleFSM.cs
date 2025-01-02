using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class M2IdleFSM : BaseState
{
    public M2IdleFSM(FSMMinion2EnemySM stateEnemy) : base("IdleState", stateEnemy) { }
    public IAiAvoid ai;
    private bool ready;

    public override void Enter()
    {
        ai = ((FSMMinion2EnemySM)stateMachine).ai;
        ai.canMove = false;
        ready = false;
    }

    public override void UpdateLogic()
    {
        var state = (FSMMinion2EnemySM)stateMachine;
        if ((Vector2.Distance(ai.position, ai.targetTransform.position) < state.visRange || state.attacking) && !ready)
        {
            ready = true;
            Reay().Forget();
        }
    }

    public async UniTask Reay()
    {
        var state = (FSMMinion2EnemySM)stateMachine;
        await UniTask.WaitForSeconds(1);
        ChangState(state.attack);
    }
}
