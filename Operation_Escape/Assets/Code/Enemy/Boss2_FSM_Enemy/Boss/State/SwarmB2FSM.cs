using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SwarmB2FSM : BaseState
{
    public SwarmB2FSM(FSMBoss2EnemySM stateMachine) : base("Swarm", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    private bool final;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        Attack().Forget();
        ai.canMove = false;
        final = false;
    }

    public async UniTaskVoid Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            ani.ChangeAnimationAttack("UnderGround");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            state.Jump(state.jumpCenter.position);

            state.SpawnEgg();
            await UniTask.WaitForSeconds(3f);
            final = true;
            await UniTask.WaitUntil(() => !final, cancellationToken: token);

            //ani.ChangeAnimationAttack("UnderGroundUP");
            //await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            //ani.ChangeAnimationAttack("Wait");
            await UniTask.WaitForSeconds(0.5f);

            ChangState(state.eat);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public override void UpdateLogic()
    {
        if (FSMBoss2EnemySM.minionHave.Count <= 0 && final)
        {
            final = false;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
