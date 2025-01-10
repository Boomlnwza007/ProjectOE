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

        try
        {
            ai.canMove = false;
            state.Run(3);
            await state.PreAttackN("PreDashAttack", 1f);
            state.animator.isFacing = false;
            Vector3 target = ai.targetTransform.position;
            var col = state.gameObject.GetComponent<Collider2D>();
            col.enabled = false;
            ai.destination = target;
            //ai.stopRadiusOn = false;
            ai.canMove = true;
            await UniTask.WaitUntil(() => ai.endMove, cancellationToken: token);
            await Attack();
            state.animator.ChangeAnimationAttack("Tired");
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            state.animator.ChangeAnimationAttack("Normal");
            state.animator.isFacing = true;
            ai.canMove = true;
            ((FSMMEnemySM)stateMachine).ChangState(((FSMMEnemySM)stateMachine).CheckDistance);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTask Attack()
    {
        var state = (FSMMEnemySM)stateMachine;
        ai.canMove = false;
        ai.monVelocity = Vector2.zero;
        state.Walk();
        await state.Attack("DashAttack", 0.4f);
        state.animator.ChangeAnimationAttack("Normal");
        state.cooldown = true;
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
