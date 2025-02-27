using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SwarmB2FSM : BaseState
{
    public SwarmB2FSM(FSMBoss2EnemySM stateMachine) : base("Swarm", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    private bool final;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        Attack().Forget();
        ai.canMove = false;
        final = false;
    }

    public async UniTaskVoid Attack() // Pass the CancellationToken
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        //var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            await UniTask.WaitForSeconds(1f);
            state.spriteBoss.enabled = false;
            state.colliderBoss.enabled = false;

            state.SpawnEgg();
            await UniTask.WaitForSeconds(3f);
            final = true;
            await UniTask.WaitUntil(() => !final, cancellationToken: token);

            ai.canMove = true;

            state.spriteBoss.enabled = true;
            state.colliderBoss.enabled = true;

            ChangState(state.eat);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public override void UpdateLogic()
    {
        if (FSMBoss2EnemySM.minionHave <= 0 && final)
        {
            final = false;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
