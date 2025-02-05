using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class M1AttackFSM : BaseState
{
    public M1AttackFSM(FSMMinion1EnemySM stateMachine) : base("Attacck", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMMinion1EnemySM)stateMachine).ai;
        ai.canMove = false;
        Attack().Forget();
    }

    public async UniTaskVoid Attack() // Pass the CancellationToken
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMMinion1EnemySM)stateMachine;
        //var ani = state.animator;
        bool hasAttacked = false;

        try
        {
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            ai.destination = CalculateDestination(ai.position, ai.targetTransform.position, state.jumpLength, state.raycastMaskWay);
            state.Walk();
            ai.canMove = true;
            state.Run(3);
            float time = 0f;

            while (time < 10 && !hasAttacked)//Edit Time Run 
            {
                time += Time.deltaTime;
                if (Vector2.Distance(ai.destination, ai.position) < 2f && ai.endMove)
                {

                    ai.canMove = false;
                    ai.monVelocity = Vector2.zero;
                    state.Walk();
                    state.Attack();

                    hasAttacked = true;

                    return;
                }

                Collider2D[] colliders = Physics2D.OverlapCircleAll(ai.position, 2f, state.raycastMask);
                foreach (var hit in colliders)
                {
                    if (hit.gameObject != state.gameObject)
                    {
                        ai.canMove = false;
                        ai.monVelocity = Vector2.zero;
                        state.Walk();
                        state.Attack();


                        hasAttacked = true;

                        return;
                    }
                }
                token.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }

            ai.canMove = true;
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            ChangState(state.checkDistance);

        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public Vector2 CalculateDestination(Vector2 currentPosition, Vector2 targetPosition, float jumpLength, LayerMask mask)
    {
        Vector2 direction = (targetPosition - currentPosition).normalized;
        RaycastHit2D[] raycast = Physics2D.RaycastAll(currentPosition, direction, jumpLength, mask);

        foreach (var hit in raycast)
        {
            if (hit.collider != null && hit.collider.gameObject != stateMachine.gameObject)
            {
                return hit.point;
            }
        }

        Debug.DrawLine(currentPosition, currentPosition + (direction * jumpLength), Color.green, 2f);
        return currentPosition + (direction * jumpLength);
    }

    public async UniTask Charge()
    {
        var token = cancellationToken.Token;
        var state = (FSMSEnemySM)stateMachine;
        bool hasAttacked = false;
        var ani = state.animator;

        state.Walk();
        ai.canMove = true;
        state.Run(5);
        ani.animator.speed = 1;
        float time = 0f;

        while (time < 10 && !hasAttacked)//Edit Time Run 
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.destination, ai.position) < 2f && ai.endMove)
            {
                //Debug.Log("ai.endMove");

                ai.canMove = false;
                ai.monVelocity = Vector2.zero;
                state.Walk();
                ani.ChangeAnimationAttack("Dash");
                Debug.Log("Attack");

                hasAttacked = true;
                //state.animator.isFacing = true;
                break;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(ai.position, 2f, state.raycastMask);
            foreach (var hit in colliders)
            {
                if (hit.gameObject != state.gameObject)
                {
                    Debug.Log(hit.name + "hit 2 ");

                    ai.canMove = false;
                    ai.monVelocity = Vector2.zero;
                    state.Walk();
                    ani.ChangeAnimationAttack("Dash");
                    Debug.Log("Attack");

                    hasAttacked = true;
                    //state.animator.isFacing = true;
                    break;
                }
            }
            token.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
