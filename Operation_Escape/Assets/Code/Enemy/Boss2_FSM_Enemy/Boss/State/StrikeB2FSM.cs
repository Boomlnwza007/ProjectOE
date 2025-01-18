using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StrikeB2FSM : BaseState
{
    public StrikeB2FSM(FSMBoss2EnemySM stateMachine) : base("Strike", stateMachine) { }
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
        var state = (FSMBoss2EnemySM)stateMachine;
        //var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            await UniTask.WaitForSeconds(3, cancellationToken: token);
            await Strike(token);
            RandomEdge();
            for (int i = 0; i < 3; i++)
            {
                await UniTask.WaitForSeconds(2);
                await Strike(token);
            }
            ChangState(state.eat);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTask Strike(CancellationToken token)
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        state.inRoom = true;
        Vector2 dir = (ai.targetTransform.position - ai.position).normalized;
        state.colliderBoss.enabled = false;            
        state.rb.velocity = dir * state.Speed;
        await UniTask.WaitUntil(() => !state.inRoom, cancellationToken: token);
    }

    public void RandomEdge()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        int rEdge = Random.Range(0, 4);
        int rNumber = 0;
        Vector3 pos;
        switch (rEdge)
        {
            case 0 :
                rNumber = Random.Range(0, state.areaMark.top.Length);
                pos = state.areaMark.top[rNumber].position;
                pos.x += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
            case 1:
                rNumber = Random.Range(0, state.areaMark.down.Length);
                pos = state.areaMark.down[rNumber].position;
                pos.x += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
            case 2:
                rNumber = Random.Range(0, state.areaMark.left.Length);
                pos = state.areaMark.left[rNumber].position;
                pos.y += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
            case 3:
                rNumber = Random.Range(0, state.areaMark.right.Length);
                pos = state.areaMark.left[rNumber].position;
                pos.y += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
