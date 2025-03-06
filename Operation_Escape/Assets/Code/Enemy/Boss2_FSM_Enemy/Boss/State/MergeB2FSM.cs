using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MergeB2FSM : BaseState
{
    public MergeB2FSM(FSMBoss2EnemySM stateMachine) : base("Merge", stateMachine) { }
    private CancellationTokenSource cancellationToken;

    public override void Enter()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        state.phase = true;
        Change().Forget();
    }

    public async UniTask Change()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            ani.ChangeAnimationAttack("Roar");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ChangState(state.checkNext);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
