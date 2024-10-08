using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class NormalAttackEMFSM : BaseState
{
    public NormalAttackEMFSM(FSMMEnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationTokenSource;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMMEnemySM)stateMachine).ai;
        ((FSMMEnemySM)stateMachine).Walk();
        cancellationTokenSource = new CancellationTokenSource();

        Attack(cancellationTokenSource.Token).Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTransform.position;        
    }

    public async UniTaskVoid Attack(CancellationToken token)
    {
        var state = ((FSMMEnemySM)stateMachine);
        try
        {
            //Debug.Log("Preparing to attack 1 for 0.5 seconds");
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            //await state.Attack("Attack");
            //Debug.Log("Attack 1");

            //Debug.Log("Preparing to attack 2 for 0.8 seconds");
            //wait UniTask.WaitForSeconds(0.8f, cancellationToken: token);
            await state.Attack("Attack");
           // Debug.Log("Attack 2");

            //Debug.Log("Preparing to attack 3 for 1.5 seconds");
            //await UniTask.WaitForSeconds(1.5f, cancellationToken: token);          
            await state.Attack("Attack");
           // Debug.Log("Attack 3");

            await UniTask.WaitForSeconds(2f, cancellationToken: token);


            ai.canMove = true;

            stateMachine.ChangState(state.CheckDistance);
        }
        catch (OperationCanceledException)
        {

            Debug.Log("The attack was canceled.");
        }      

    }

    public override void Exit()
    {
         cancellationTokenSource.Cancel();
    }

}
