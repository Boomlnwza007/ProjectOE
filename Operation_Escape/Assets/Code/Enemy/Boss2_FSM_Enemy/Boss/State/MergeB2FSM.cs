using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MergeB2FSM : BaseState
{
    public MergeB2FSM(FSMBoss2EnemySM stateMachine) : base("Merge", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    public List<int> stateAtk = new List<int>();
    private bool final;

    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        if (stateAtk.Count <= 0)
        {
            var state = (FSMBoss2EnemySM)stateMachine;
            state.phase = true;
            ChangState(state.checkNext);
            return;
        }

        Debug.Log(stateAtk[0]+" "+ stateAtk[1]);
        Attack().Forget();
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
            await UniTask.WaitForSeconds(1f, cancellationToken: token);

            await UniTask.WhenAll(ChooseState(stateAtk[0]), ChooseState(stateAtk[1]));
            Debug.Log("allEnd");

            await UniTask.WaitForSeconds(1f, cancellationToken: token);
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
        if(FSMBoss2EnemySM.minionHave.Count <= 0 && final)
        {
            final = false;
        }
    }

    public async UniTask ChooseState(int number)
    {
        switch (number)
        {
            case 1:
                await StrikeB2FSM();
                break;
            case 2:
                await AreaAttackB2FSM();
                break;
            case 3:
                await SwarmB2FSM();
                break;
            case 4:
                await LaserB2FSM();
                break;
        }
    }

    public async UniTask SwarmB2FSM()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            state.SpawnEggP2();
            await UniTask.WaitForSeconds(3f);
            final = true;
            await UniTask.WaitUntil(() => !final, cancellationToken: token);
            Debug.Log("3+End");
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }

    }

    public async UniTask AreaAttackB2FSM()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            ani.ChangeAnimationAttack("UnderGroundUP");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Area_PreAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Area_Attacking");

            bool increasing = true;
            int radius = 5;

            for (int i = 0; i < 3; i++)
            {
                Debug.Log(1);
                for (int j = 0; j < 5; j++)
                {
                    state.SpawnParticle(radius);
                    if (increasing)
                    {
                        radius += 5;
                        if (radius > 25)
                        {
                            radius = 5;
                        }
                    }
                    await UniTask.WaitForSeconds(0.5f);
                }
            }

            ani.ChangeAnimationAttack("Area_EndAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Wait");
            await UniTask.WaitForSeconds(0.5f);
            Debug.Log("2+End");
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }

    }

    public async UniTask StrikeB2FSM()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            DummyBoss2 dummy = state.SpawnDummy();
            dummy.boss2 = state;
            await dummy.StrikeB2FSM();
            await UniTask.WaitForSeconds(0.5f);
            dummy.Die();
            Debug.Log("1+End");
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }

    }

    public async UniTask LaserB2FSM()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            for (int i = 0; i < 3; i++)
            {
                state.SpawnLaserCols(-10);
                await UniTask.WaitForSeconds(1.2f);
                state.SpawnLaserRows(8);
                await UniTask.WaitForSeconds(1.2f);
                state.SpawnLaserGrid();
                await UniTask.WaitForSeconds(1.2f);
            }
            await UniTask.WaitForSeconds(0.5f);
            Debug.Log("4+End");

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
