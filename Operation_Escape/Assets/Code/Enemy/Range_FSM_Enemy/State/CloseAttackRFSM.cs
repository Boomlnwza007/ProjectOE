using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CloseAttackRFSM : BaseState
{
    public CloseAttackRFSM(FSMREnemySM stateMachine) : base("CloseAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public bool go;
    bool dash;
    private CancellationTokenSource cancellationToken;


    // Start is called before the first frame update
    public override void Enter()
    {
        go = false;
        cancellationToken = new CancellationTokenSource();

        var state = (FSMREnemySM)stateMachine;
        if (state.cooldown)
        {
            stateMachine.ChangState(state.normalAttackState);
            return;
        }

        ai = ((FSMREnemySM)stateMachine).ai;

        Debug.Log("ตั้งท่าเตรียมโจมตี CloseAttack");
        Attack(cancellationToken.Token).Forget();

    }
    public override void UpdateLogic()
    {
        if (dash)
        {
            DashStart();
        }
    }

    public async UniTask Attack(CancellationToken token)
    {
        var state = (FSMREnemySM)stateMachine;
        try
        {
            state.animator.isFacing = false;            
            ai.canMove = false;
            Dash();
            await UniTask.WaitWhile(() => dash);
            ai.canMove = false;
            state.rb.velocity = Vector2.zero;
            await state.PreAttack("PreAttack", 0.1f);
            await state.Attack("Attack", 0.1f);
            state.FireClose();
            await UniTask.WaitForSeconds(0.2f, cancellationToken: token);
            state.animator.ChangeAnimationAttack("Normal");
            ai.canMove = true;

            state.Walk();
            state.cooldown = true;
            state.animator.isFacing = true;
            stateMachine.ChangState(state.checkDistanceState);
        }
        catch (OperationCanceledException)
        {
            return;
        }        
    }

    public Vector2 Ray()
    {
        var state = (FSMREnemySM)stateMachine;
        Vector2 dir = (ai.position- ai.targetTransform.position).normalized;
        float maxAngle = 90f;
        float currentAngle = 0f;
        float angleStep = 15f;
        bool obstacleDetected = false;
        bool turnLeft = true;
        Vector2 lastValidDirection = dir;

        while (currentAngle <= maxAngle)
        {
            RaycastHit2D[] raycast = Physics2D.RaycastAll(ai.position, dir, state.jumpLength, state.raycastMaskWay);
            obstacleDetected = false;

            if (raycast.Length > 0)
            {
                foreach (var hit in raycast)
                {
                    if (hit.collider != null && hit.collider.gameObject != state.gameObject)
                    {
                        obstacleDetected = true;
                        break;
                    }
                }
            }
            else
            {
                ai.destination = (Vector2)ai.position + (dir.normalized * state.jumpLength);
                break;
            }

            if (!obstacleDetected)
            {
                ai.destination = (Vector2)ai.position + (dir.normalized * state.jumpLength);
                lastValidDirection = dir;
                Debug.DrawLine((Vector2)ai.position, (Vector2)ai.position + (dir.normalized * state.jumpLength), Color.green);
                break;
            }
            else
            {
                currentAngle += angleStep;
                float radians = currentAngle * Mathf.Deg2Rad;
                float cos = Mathf.Cos(radians);
                float sin = Mathf.Sin(radians);

                if (turnLeft)
                {
                    // เบี่ยงซ้าย
                    dir = new Vector2(
                        dir.x * cos - dir.y * sin,
                        dir.x * sin + dir.y * cos
                    );
                }
                else
                {
                    dir = new Vector2(
                        dir.x * cos + dir.y * sin,
                        -dir.x * sin + dir.y * cos
                    );
                }

                turnLeft = !turnLeft;
                Debug.DrawLine((Vector2)ai.position, (Vector2)ai.position + (dir.normalized * state.jumpLength), Color.red);
            }
        }

        if (obstacleDetected)
        {
            bool randomTurnLeft = UnityEngine.Random.Range(0, 2) == 0;

            float finalAngle = maxAngle * Mathf.Deg2Rad;  
            float cos = Mathf.Cos(finalAngle);
            float sin = Mathf.Sin(finalAngle);

            if (randomTurnLeft)
            {
                lastValidDirection = new Vector2(
                    dir.x * cos - dir.y * sin,  
                    dir.x * sin + dir.y * cos   
                );
            }
            else
            {
                // หากสุ่มได้เบี่ยงขวาสุด
                lastValidDirection = new Vector2(
                    dir.x * cos + dir.y * sin,
                    -dir.x * sin + dir.y * cos
                );
            }

            return lastValidDirection.normalized;
        }

        return lastValidDirection.normalized;
    }

    public void Dash()
    {
        var state = ((FSMREnemySM)stateMachine);
        dash = true;
        state.rollSpeed = state.dodgeMaxSpeed;
    }

    public void DashStart()
    {
        var state = ((FSMREnemySM)stateMachine);
        Vector2 dir = (ai.position - ai.targetTransform.position ).normalized;
        //Vector2 dir = Ray();

        //RaycastHit2D[] raycast = Physics2D.RaycastAll(ai.position, dir, state.dodgeStopRange, LayerMask.GetMask("Obstacle"));
        //if (raycast.Length > 0)
        //{
        //    state.rollSpeed = state.dodgeMinimium;
        //    dash = false;
        //    state.rb.velocity = Vector3.zero;
        //    return;
        //}

        state.rollSpeed -= state.rollSpeed * state.dodgeSpeedDropMultiplier * Time.deltaTime;
        if (state.rollSpeed < state.dodgeMinimium)
        {
            dash = false;
            state.rb.velocity = Vector3.zero;
        }

        state.rb.velocity = dir * state.rollSpeed;
    }

    public override void Exit()
    {
        cancellationToken?.Cancel();
    }
}
