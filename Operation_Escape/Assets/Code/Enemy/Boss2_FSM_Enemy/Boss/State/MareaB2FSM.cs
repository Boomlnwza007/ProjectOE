using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MareaB2FSM : BaseState
{
    public MareaB2FSM(FSMBoss2EnemySM stateMachine) : base("AreaMerge", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    public List<int> rNumber = new List<int> {3, 4 };

    // Start is called before the first frame update
    public override void Enter()
    {
        cancellationToken = new CancellationTokenSource();
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        ai.canMove = false;
        if (rNumber.Count == 0)
        {
            rNumber = new List<int> {3, 4 };
        }
        int index = Random.Range(0, rNumber.Count);
        int selectedAttack = rNumber[index];
        rNumber.RemoveAt(index);
        ChooseState(selectedAttack).Forget();
    }

    public async UniTaskVoid ChooseState(int number)
    {
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

            switch (number)
            {
                case 3:
                    await Attack3();
                    break;
                case 4:
                    await Attack4();
                    break;
            }

            ani.ChangeAnimationAttack("Area_EndAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Wait");
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            ChangState(state.eat);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTask Attack3()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;

        try
        {
            state.SpawnEggP2();
            float radius = 4.5f;

            ani.ChangeAnimationAttack("UnderGroundUP");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Area_PreAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Area_Attacking");
            await UniTask.WaitForSeconds(0.5f);

            while (FSMBoss2EnemySM.minionHave.Count > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        state.SpawnLightning();
                        state.SpawnParticle(radius);
                        radius += 5;
                        if (radius > 25)
                        {
                            radius = 5;
                        }
                        await UniTask.WaitForSeconds(0.25f, cancellationToken: token);
                        state.sound.PlayMonAtk(2);
                        await UniTask.WaitForSeconds(0.25f, cancellationToken: token);
                    }
                }
                await UniTask.Yield(cancellationToken: token);
            }

            await UniTask.WaitForSeconds(1f, cancellationToken: token);

        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }
    public async UniTask Attack4()
    {
        cancellationToken = new CancellationTokenSource();
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

            float radius = 4.5f;
            for (int i = 0; i < 3; i++)
            {
                await LaserB2FSM();
                for (int j = 0; j < 5; j++)
                {
                    state.SpawnLightning();
                    state.SpawnParticle(radius);
                    radius += 5;
                    if (radius > 25)
                    {
                        radius = 5;
                    }
                    await UniTask.WaitForSeconds(0.25f, cancellationToken: token);
                    state.sound.PlayMonAtk(2);
                    await UniTask.WaitForSeconds(0.25f, cancellationToken: token);
                }
            }

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
        var sound = state.sound;

        try
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
            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
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
