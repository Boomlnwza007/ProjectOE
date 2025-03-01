using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

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
                await state.PreAttackN("PreDashAttack");
                ani.ChangeSortingLayer("TileMapON");
                state.col.enabled = false;
                state.shadow.SetActive(false);
                jump = true;
                startPos = ai.position;
                List<Vector3> vectors = CheckObjectsInArea(PlayerControl.control.transform.position,3,6,3, state.raycastMask);
                if (vectors.Count != 0)
                {
                    target = vectors[Random.Range(0, vectors.Count)];
                }

                controlPoint = (startPos + (Vector2)ai.targetTransform.position) / 2 + Vector2.up * 3;
                await UniTask.WaitUntil(() => !jump , cancellationToken: token);
                state.col.enabled = true;
                state.shadow.SetActive(true);       
                ani.ChangeSortingLayer("Player");

                await state.Attack("DashAttack",1f);
                state.animator.ChangeAnimationAttack("Normal");
                state.cooldown = true;
                ai.canMove = true;
                ani.isFacing = true;
                ChangState(state.CheckDistance); 
                return;
            }


            ai.canMove = true;
            ChangState(state.CheckDistance);
        }
        catch (System.OperationCanceledException)
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
        if (raycast.collider != null && raycast.collider.CompareTag("Player"))
        {
            target = raycast.collider.gameObject.transform.position;
            state.animator.isFacing = false;
        }
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

    public List<Vector3> CheckObjectsInArea(Vector2 center, float width, float height, int gridSize, LayerMask layerMask)
    {
        float gridWidth = width / gridSize;
        float gridHeight = height / gridSize;
        List<Vector3> vectors = new List<Vector3>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 checkPosition = center + new Vector2(
                    (x - (gridSize - 1) / 2f) * gridWidth,
                    (y - (gridSize - 1) / 2f) * gridHeight
                );
                Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPosition, new Vector2(gridWidth, gridHeight), 0f, layerMask);
                Color debugColor = colliders.Length == 0 ? Color.green : Color.red;

                if (colliders.Length == 0)
                {
                    vectors.Add(checkPosition);
                }
                DrawDebugBox(checkPosition, gridWidth, gridHeight, debugColor);
            }
        }

        return vectors;
    }

    private void DrawDebugBox(Vector2 center, float width, float height, Color color)
    {
        Vector3 bottomLeft = center - new Vector2(width / 2, height / 2);
        Vector3 bottomRight = center + new Vector2(width / 2, -height / 2);
        Vector3 topLeft = center + new Vector2(-width / 2, height / 2);
        Vector3 topRight = center + new Vector2(width / 2, height / 2);

        Debug.DrawLine(bottomLeft, bottomRight, color, 0.1f);
        Debug.DrawLine(bottomRight, topRight, color, 0.1f);
        Debug.DrawLine(topRight, topLeft, color, 0.1f);
        Debug.DrawLine(topLeft, bottomLeft, color, 0.1f);
    }
    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
