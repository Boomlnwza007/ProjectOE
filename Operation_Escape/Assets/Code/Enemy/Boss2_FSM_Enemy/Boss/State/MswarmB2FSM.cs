using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MswarmB2FSM : BaseState
{
    public MswarmB2FSM(FSMBoss2EnemySM stateMachine) : base("SwarmMerge", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    public List<int> rNumber = new List<int> { 2, 3, 4 };

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        ai.canMove = false;
        Attack().Forget();
    }

    public async UniTaskVoid Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        var sound = state.sound;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            ani.ChangeAnimationAttack("UnderGround");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            state.Jump(state.jumpCenter.position);
            state.SpawnEggP2(4, 2,1);

            for (int i = 0; i < 3; i++)
            {
                state.SpawnLaserCols(-10);
                sound.PlayPreAtk(9);
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                sound.PlayMonAtk(3);
                await UniTask.WaitForSeconds(0.2f, cancellationToken: token);

                state.SpawnLaserRows(8);
                sound.PlayPreAtk(9);
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                sound.PlayMonAtk(3);
                await UniTask.WaitForSeconds(0.2f, cancellationToken: token);

                state.SpawnLaserGrid();
                sound.PlayPreAtk(9);
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                sound.PlayMonAtk(3);
                await UniTask.WaitForSeconds(0.2f, cancellationToken: token);
            }

            await UniTask.WaitForSeconds(1.5f, cancellationToken: token);

            while (FSMBoss2EnemySM.minionHave.Count > 0)
            {
                await UniTask.Yield(cancellationToken: token);
            }

            await UniTask.WaitForSeconds(0.5f,cancellationToken: token);
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
