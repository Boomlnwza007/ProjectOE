using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class IdleB2FSM : BaseState
{
    public IdleB2FSM(FSMBoss2EnemySM stateMachine) : base("Idle", stateMachine) { }
    public IAiAvoid ai;
    private bool ready;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        ai.destination = ai.position;
        ai.canMove = false;
        ready = false;
    }
    public override void UpdateLogic()
    {
        if (((FSMBoss2EnemySM)stateMachine).attacking && !ready)
        {
            ready = true;
            Reay().Forget();
        }
    }

    public async UniTask Reay()
    {
        var ani = ((FSMBoss2EnemySM)stateMachine).animator;
        if (!((FSMBoss2EnemySM)stateMachine).first)
        {
            ani.ChangeAnimationAttack("Roar");
            await UniTask.WaitUntil(() => ani.endAnim);
            ani.ChangeAnimationAttack("Wait");
        }
        await UniTask.WaitForSeconds(0.5f);
        ai.canMove = true;
        stateMachine.ChangState(((FSMBoss2EnemySM)stateMachine).checkNext);
    }
}
