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
    private Vector2 startPos;
    private Vector2 controlPoint;
    private Vector2 target;
    private bool jump;
    private float t = 0f;



    // Start is called before the first frame update
    public override void Enter()
    {
        cancellationToken = new CancellationTokenSource();
        ai = ((FSMMEnemySM)stateMachine).ai;
        ((FSMMEnemySM)stateMachine).Walk();
        t = 0;

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
        var ani = state.animator;

        try
        {
            ai.canMove = false;
            if (CheckWay())
            {
                ani.isFacing = false;
                await state.PreAttackN("PreDashAttack");
                state.col.enabled = false;
                state.shadow.SetActive(false);
                jump = true;
                startPos = ai.position;
                target = ai.targetTransform.position;
                controlPoint = (startPos + (Vector2)ai.targetTransform.position) / 2 + Vector2.up * 3;
                await UniTask.WaitUntil(() => !jump , cancellationToken: token);
                state.col.enabled = true;
                state.shadow.SetActive(true);
                ani.isFacing = true;
                await state.Attack("DashAttack",1f);
                state.animator.ChangeAnimationAttack("Normal");
                state.cooldown = true;
            }


            ai.canMove = true;
            ChangState(state.CheckDistance);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    public override void UpdatePhysics()
    {
        if (jump)
        {
            t += Time.deltaTime * 1;
            t = Mathf.Clamp01(t);

            Vector2 pos = Mathf.Pow(1 - t, 2) * startPos +
                          2 * (1 - t) * t * controlPoint +
                          Mathf.Pow(t, 2) * (Vector2)target;
            var state = (FSMMEnemySM)stateMachine;

            state.gameObject.transform.position = pos;

            if (Vector2.Distance(ai.position,target) < 1f)
            {
                jump = false;
                t = 0;
            }
        }
    }

    public bool CheckWay()
    {
        var state = (FSMMEnemySM)stateMachine;
        Vector2 dir = (ai.targetTransform.position - ai.position).normalized;
        RaycastHit2D raycast = Physics2D.Raycast(ai.position, dir, state.jumpLength, state.raycastMaskWay);
        target = raycast.collider.gameObject.transform.position;
        return raycast.collider != null && raycast.collider.CompareTag("Player");
    }   

    public async UniTask Attack()
    {
        var state = (FSMMEnemySM)stateMachine;
        ai.canMove = false;
        ai.monVelocity = Vector2.zero;
        state.Walk();
        await state.Attack("DashAttack",1);
        state.animator.ChangeAnimationAttack("Normal");
        state.cooldown = true;
        Debug.Log("A12");
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
