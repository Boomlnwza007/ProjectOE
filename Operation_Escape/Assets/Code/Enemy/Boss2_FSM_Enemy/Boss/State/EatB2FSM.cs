using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EatB2FSM : BaseState
{
    public EatB2FSM(FSMBoss2EnemySM stateMachine) : base("Eat", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        ai.canMove = false;
        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        ai.destination = ai.targetTransform.position;
    }

    public async UniTaskVoid Attack() // Pass the CancellationToken
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);
        ai.maxspeed = state.speedEat;

        try
        {
            Debug.Log("Eat");
            await UniTask.WaitForSeconds(1 , cancellationToken : token);
            state.spriteBoss.enabled = false;
            state.colliderBoss.enabled = false;
            state.Jump(ai.targetTransform.position);
            ai.canMove = true;
            ani.ChangeAnimationAttack("Eat");
            await UniTask.WaitUntil(() => ani.endAnim , cancellationToken : token);
            ai.canMove = false;
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            ai.maxspeed = state.Speed;
            ani.ChangeAnimationAttack("Wait");
            state.spriteBoss.enabled = true;
            state.colliderBoss.enabled = true;
            await UniTask.WaitForSeconds(5f, cancellationToken: token);
            ai.canMove = true;
            ChangState(state.checkNext);
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
