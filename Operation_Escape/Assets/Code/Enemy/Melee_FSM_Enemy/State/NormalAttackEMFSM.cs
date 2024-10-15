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
            state.Run(0.2f);
            ai.canMove = false;
            state.animator.isFacing = false;
            await state.PreAttack("PreAttack", 0.5f);
            await state.Attack("Attack", 0.3f);
            if (Distance())
            {
                state.animator.ChangeAnimationAttack("Normal");
                await UniTask.WaitForSeconds(1f); Debug.Log("Far");
                ai.canMove = true;
                state.animator.isFacing = true;
                state.Walk();
                stateMachine.ChangState(state.CheckDistance);
                return;
            }  
            
            ai.canMove = true;
            state.animator.isFacing = true;
            await UniTask.DelayFrame(1);

            ai.canMove = false;
            state.animator.isFacing = false;
            await state.PreAttack("PreAttack", 0.8f);
            await state.Attack("Attack", 0.3f);
            if (Distance())
            {
                state.animator.ChangeAnimationAttack("Normal");
                await UniTask.WaitForSeconds(1f); Debug.Log("Far");
                ai.canMove = true;
                state.animator.isFacing = true;
                state.Walk();
                stateMachine.ChangState(state.CheckDistance);
                return;
            }

            ai.canMove = true;
            state.animator.isFacing = true;
            await UniTask.DelayFrame(1);

            ai.canMove = false;
            state.animator.isFacing = false;
            await state.PreAttack("PreAttack", 1.5f);
            await state.Attack("Attack", 0.3f);
            ai.canMove = true;
            state.animator.isFacing = true;

            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            ai.canMove = true;
            state.Walk();
            stateMachine.ChangState(state.CheckDistance);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("The attack was canceled.");
            return;
        }

    }

    public bool Distance()
    {
        if (Vector2.Distance(ai.targetTransform.position, ai.position) > 3f)
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
         cancellationTokenSource.Cancel();
    }

}
