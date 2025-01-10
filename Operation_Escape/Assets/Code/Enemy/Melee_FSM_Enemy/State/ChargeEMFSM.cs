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
        state.Run(2);
        time = 0f;

        try
        {
            ai.canMove = false;
            await state.PreAttackN("PreDashAttack", 0.5f);
            Vector2 dir = (ai.targetTransform.position - ai.position).normalized;
            RaycastHit2D[] raycast = Physics2D.RaycastAll(ai.position, dir, dir.magnitude, state.raycastMaskWay);
            if (raycast.Length > 0)
            {
                foreach (var hit in raycast)
                {
                    if (hit.collider != null && hit.collider.gameObject != state.gameObject)
                    {
                        //Debug.Log(hit.collider.name + " hit");
                        ai.destination = hit.point;
                        break;
                    }
                    else
                    {
                        ai.destination = (Vector2)ai.position + (dir.normalized * dir.magnitude);
                        Debug.DrawLine((Vector2)ai.position, (Vector2)ai.position + (dir.normalized * dir.magnitude), Color.red);  
                        //Debug.Log("no hit");                       
                    }
                }
            }
            else
            {
                ai.destination = (Vector2)ai.position + (dir.normalized * dir.magnitude);
                //Debug.DrawLine((Vector2)ai.position, (Vector2)ai.position + (dir.normalized * state.jumpLength), Color.red);
            }


            ai.canMove = true;
            bool hasAttacked = false;
            state.animator.isFacing = false;

            while (time < 10 && !hasAttacked)//Edit Time Run 
            {
                time += Time.deltaTime;
                if (Vector2.Distance(ai.destination, ai.position) < 2f && ai.endMove)
                {
                    //Debug.Log("ai.endMove");
                    await Attack();
                    hasAttacked = true;
                    state.animator.isFacing = true;
                    break;
                }

                Collider2D[] colliders = Physics2D.OverlapCircleAll(ai.position, 2f, state.raycastMask);
                foreach (var hit in colliders)
                {
                    if (hit.gameObject != state.gameObject)
                    {
                        Debug.Log(hit.name + "hit 2 ");
                        await Attack();
                        hasAttacked = true;     
                        state.animator.isFacing = true;
                        break;
                    }
                }
                token.ThrowIfCancellationRequested();
                await UniTask.Yield();               
            }

            //state.animator.isFacing = true;
            state.Walk();
            ai.canMove = false;
            Debug.Log("Stop");
            state.animator.ChangeAnimationAttack("Tired");
            await UniTask.WaitForSeconds(3f, cancellationToken: token);//Edit time Stop
            state.animator.ChangeAnimationAttack("Normal");
            ai.canMove = true;
            state.animator.isFacing = true;
            Debug.Log("End");
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
        Debug.Log("A12");
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
