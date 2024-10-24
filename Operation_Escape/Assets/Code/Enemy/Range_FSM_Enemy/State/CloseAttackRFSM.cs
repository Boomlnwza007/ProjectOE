using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackRFSM : BaseState
{
    public CloseAttackRFSM(FSMREnemySM stateMachine) : base("CloseAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public bool go;
    float time;

    // Start is called before the first frame update
    public override void Enter()
    {
        go = false;
        time = 0;

        var state = (FSMREnemySM)stateMachine;
        if (state.cooldown)
        {
            stateMachine.ChangState(state.normalAttackState);
            return;
        }

        ai = ((FSMREnemySM)stateMachine).ai;
        //speed = ai.Maxspeed;
        //time = 0;
        //ai.Maxspeed = speed * 10;
        Debug.Log("ตั้งท่าเตรียมโจมตี CloseAttack");
        Attack().Forget();
        //if (state.rb != null)
        //{
        //    ai.canMove = false;
        //    Vector2 knockbackDirection = (ai.position - ai.targetTransform.position).normalized;
        //    Debug.DrawLine(ai.position, ai.position + (ai.position - ai.targetTransform.position).normalized * 10);
        //    state.rb.AddForce(knockbackDirection * 150, ForceMode2D.Impulse);

        //    Debug.Log("back");
        //}
        //ai.destination = ai.position + (ai.position - ai.targetTarnsform.position).normalized * 5;
    }

    public override void UpdateLogic()  
    {
        //if (!go)
        //{
        //    time += Time.deltaTime;
        //    if (time > 0.5f)
        //    {
        //        ((FSMREnemySM)stateMachine).rb.velocity = Vector2.zero;
        //        ((FSMREnemySM)stateMachine).FireClose();
        //        ai.canMove = true;
        //        go = true;
        //    }
        //}
        //else
        //{
        //    time += Time.deltaTime;
        //    if (time > 0.5f)
        //    {
        //        ((FSMREnemySM)stateMachine).cooldown = true;
        //        stateMachine.ChangState(((FSMREnemySM)stateMachine).checkDistanceState);
        //    }
        //}

    }

    public async UniTask Attack()
    {
        var state = (FSMREnemySM)stateMachine;
        state.animator.isFacing = false;
        state.Run(2);
        Ray();
        ai.canMove = true;
        bool hasAttacked = false;       

        while (time < 3 && !hasAttacked)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.destination, ai.position) < 3f && ai.endMove)
            {
                Debug.Log(0);
                state.Walk();
                hasAttacked = true;
                state.animator.isFacing = true;
                break;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(ai.position, 2f, state.raycastMask);
            foreach (var hit in colliders)
            {
                if (hit.gameObject != state.gameObject)
                {
                    Debug.Log(hit.gameObject.name);
                    state.Walk();
                    hasAttacked = true;
                    state.animator.isFacing = true;
                    break;
                }
            }
            await UniTask.Yield();
        }

        ai.canMove = false;
        state.rb.velocity = Vector2.zero;
        await state.PreAttack("PreAttack", 0.1f);
        await state.Attack("Attack", 0.1f);
        state.FireClose();
        await UniTask.WaitForSeconds(0.2f); ;
        state.animator.ChangeAnimationAttack("Normal");
        ai.canMove = true;

        state.Walk();
        state.cooldown = true;        
        stateMachine.ChangState(state.checkDistanceState);
    }

    public void Ray()
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
            bool randomTurnLeft = Random.Range(0, 2) == 0;

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

            ai.destination = (Vector2)ai.position + (lastValidDirection.normalized * state.jumpLength);
            Debug.DrawLine((Vector2)ai.position, (Vector2)ai.position + (lastValidDirection.normalized * state.jumpLength), Color.blue);
        }

        Debug.DrawLine((Vector2)ai.position, (Vector2)ai.position + (lastValidDirection.normalized * state.jumpLength), Color.blue);

    }
}
