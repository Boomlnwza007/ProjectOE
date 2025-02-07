using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AtkCloseB1FSM : BaseState
{
    public AtkCloseB1FSM(FSMBoss1EnemySM stateMachine) : base("AtkClose", stateMachine) { }
    private CancellationTokenSource cancellationToken;
    public IAiAvoid ai;
    private int count;

    public override void Enter()
    {
        var state = (FSMBoss1EnemySM)stateMachine;
        cancellationToken = new CancellationTokenSource();
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        ai.canMove = true;
        Attack().Forget();
        count++;
        if (count > 3)
        {
            state.normalAState.maxCountATK = Random.Range(2,3);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;
    }

    public async UniTaskVoid Attack()
    {
        var state = ((FSMBoss1EnemySM)stateMachine);
        var ani = ((FSMBoss1EnemySM)stateMachine).boss1AniControl;
        var token = cancellationToken.Token;
        try
        {
            ai.canMove = false;
            ani.ChangeAnimationAttack("StartAtkClose");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("AtkClose");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            ani.ChangeAnimationAttack("Wait");
            ai.canMove = true;
            ChangState(((FSMBoss1EnemySM)stateMachine).checkDistanceState);
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    public override void Exit()
    {
        cancellationToken?.Cancel();
    }
}
