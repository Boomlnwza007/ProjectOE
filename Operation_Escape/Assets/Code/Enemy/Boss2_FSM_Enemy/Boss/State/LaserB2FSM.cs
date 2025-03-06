using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LaserB2FSM : BaseState
{
    public LaserB2FSM(FSMBoss2EnemySM stateMachine) : base("Laser", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        ai.canMove = false;
        Attack().Forget();
    }

    public async UniTask Attack()
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

            for (int i = 0; i < 3; i++)
            {
                state.SpawnLaserCols(-10);
                await UniTask.WaitForSeconds(1.2f, cancellationToken: token);
                state.SpawnLaserRows(8);
                await UniTask.WaitForSeconds(1.2f, cancellationToken: token);
                state.SpawnLaserGrid();
                await UniTask.WaitForSeconds(1.2f, cancellationToken: token);
            }

            //ani.ChangeAnimationAttack("UnderGroundUP");
            //await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            //ani.ChangeAnimationAttack("Wait");
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            ChangState(state.eat);

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
