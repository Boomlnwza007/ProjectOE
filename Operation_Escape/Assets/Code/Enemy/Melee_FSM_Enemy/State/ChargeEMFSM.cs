using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class ChargeEMFSM : BaseState
{
    public ChargeEMFSM(FSMMEnemySM stateMachine) : base("Charge", stateMachine) { }
    public IAiAvoid ai;
    float time;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        cancellationToken = new CancellationTokenSource();
        ai = ((FSMMEnemySM)stateMachine).ai;
        ((FSMMEnemySM)stateMachine).Walk();


        if (!((FSMMEnemySM)stateMachine).cooldown)
        {
            Debug.Log("ตั้งท่าเตรียมโจมตี");
            Attack(cancellationToken.Token).Forget();
        }
        else
        {
            ((FSMMEnemySM)stateMachine).ChangState(((FSMMEnemySM)stateMachine).CheckDistance);
        }      
        
    }

    public async UniTaskVoid Attack(CancellationToken token) // Pass the CancellationToken
    {
        var state = (FSMMEnemySM)stateMachine;
        ai.randomDeviation = false;
        state.Run(2);
        time = 0f;

        try
        {
            while (time < 2f)//Edit Time Run 
            {
                time += Time.deltaTime;

                if (Vector2.Distance(ai.position, ai.targetTransform.position) < 2f)//Edit Distance
                {
                    ai.monVelocity = Vector2.zero;
                    state.Walk();
                    //await UniTask.WaitForSeconds(0.5f, cancellationToken: token);//Edit time PreAttack
                    await state.Attack("Attack");
                    Debug.Log("A1");
                    state.cooldown = true;
                    break;
                }

                token.ThrowIfCancellationRequested();
                await UniTask.Yield();               
            }

            state.Walk();
            ai.canMove = false;
            Debug.Log("Stop");
            await UniTask.WaitForSeconds(3f, cancellationToken: token);//Edit time Stop
            ai.canMove = true;
            Debug.Log("End");
            ((FSMMEnemySM)stateMachine).ChangState(((FSMMEnemySM)stateMachine).CheckDistance);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
