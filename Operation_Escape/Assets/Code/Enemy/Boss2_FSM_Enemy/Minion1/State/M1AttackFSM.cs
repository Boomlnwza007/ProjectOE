using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class M1AttackFSM : BaseState
{
    public M1AttackFSM(FSMMinion1EnemySM stateMachine) : base("Attacck", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    public bool cooldown;
    private Vector2 startPos;
    private Vector2 controlPoint;
    private Vector2 target;
    private bool jump;
    private float t = 0f;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMMinion1EnemySM)stateMachine).ai;
        ai.canMove = false;
        t = 0f;
        Attack().Forget();

    }

    public override void UpdatePhysics()
    {
        if (jump)
        {
            t += Time.deltaTime * 2f;
            t = Mathf.Clamp01(t);

            Vector2 pos = Mathf.Pow(1 - t, 2) * startPos +
                          2 * (1 - t) * t * controlPoint +
                          Mathf.Pow(t, 2) * (Vector2)target;
            var state = (FSMMinion1EnemySM)stateMachine;

            state.gameObject.transform.position = pos;

            if (Vector2.Distance(ai.position, target) < 0.5f)
            {
                jump = false;
                t = 0;
            }
        }
    }

    public async UniTaskVoid Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMMinion1EnemySM)stateMachine;
        var ani = state.animator;

        try
        {
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            state.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TileMapON";
            state.Jump(false);
            ani.ChangeAnimationAttack("jump");
            jump = true;
            startPos = ai.position;
            state.animator.isFacing = false;
            target = PlayerControl.control.transform.position;

            controlPoint = (startPos + (Vector2)ai.targetTransform.position) / 2 + Vector2.up * 4;
            await UniTask.WaitUntil(() => !jump, cancellationToken: token);
            state.Jump(true);
            state.Attack();            
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
