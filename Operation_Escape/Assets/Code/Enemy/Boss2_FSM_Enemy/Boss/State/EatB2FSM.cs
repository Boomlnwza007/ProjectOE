using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EatB2FSM : BaseState
{
    public EatB2FSM(FSMBoss2EnemySM stateMachine) : base("Eat", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        Attack().Forget();
    }

    public async UniTaskVoid Attack() // Pass the CancellationToken
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMMinion1EnemySM)stateMachine;
        //var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            await UniTask.Delay(1);


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
