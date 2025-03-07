using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MstrikeB2FSM : BaseState
{
    public MstrikeB2FSM(FSMBoss2EnemySM stateMachine) : base("StrikeMerge", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    public List<int> rNumber = new List<int> { 2, 3, 4 };
    public List<int> spawnPoint = new List<int> {0, 1, 2, 3};

    private DummyBoss2 dummy;

    // Start is called before the first frame update
    public override void Enter()
    {
        cancellationToken = new CancellationTokenSource();
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        ai.canMove = false;
        if (rNumber.Count == 0)
        {
            rNumber = new List<int> {2, 3, 4 };
        }
        int index = Random.Range(0, rNumber.Count);
        int selectedAttack = rNumber[index];
        rNumber.RemoveAt(index);
        ChooseState(selectedAttack).Forget();
        Debug.Log(1 + " " + selectedAttack);
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
            dummy = state.SpawnDummy();
            dummy.boss2 = state;

            switch (number)
            {
                case 2:
                    await Attack2();
                    break;
                case 3:
                    spawnPoint = new List<int> { 0, 1, 2, 3 };
                    await Attack3();
                    break;
                case 4:
                    await Attack4();
                    break;
            }

            await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
            ChangState(state.eat);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }        
    }

    public async UniTask Attack2()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            dummy.sound.PlayPreAtk(0);
            dummy.ani.ChangeAnimationAttack("Strike");
            await UniTask.WaitUntil(() => dummy.ani.endAnim, cancellationToken: token);
            await UniTask.WhenAll(dummy.StrikeAtk(), Lightning());
            dummy.RandomEdge();
            dummy.rb.velocity = Vector2.zero;

            for (int i = 0; i < 4; i++)
            {
                await UniTask.WhenAll(dummy.StrikeAtk(), Lightning());
            }

            await UniTask.WaitForSeconds(0.5f);
            dummy.Die();
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTask Lightning()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        var token = cancellationToken.Token;

        //await UniTask.WaitForSeconds(0.2f, cancellationToken: token);
        await UniTask.WaitUntil(() => dummy.inRoom, cancellationToken: cancellationToken.Token);
        while (dummy.inRoom)
        {
            Vector2 dropPosition;
            for (int i = 0; i < 10; i++)
            {
                await UniTask.WaitForSeconds(0.05f, cancellationToken: token);
                dropPosition = (Vector2)dummy.gameObject.transform.position + Random.insideUnitCircle * 8f;
                state.SpawnLightning(dropPosition);
            }
            //await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
        }        
    }

    public async UniTask Attack3()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            await dummy.StrikeB2FSM();
            state.SpawnEggPos(RandomPos());
            for (int i = 0; i < 3; i++)
            {
                await UniTask.WaitForSeconds(2f, cancellationToken: token);
                state.SpawnEggPos(RandomPos());
                await dummy.StrikeAtk();
            }

            await UniTask.WaitForSeconds(2f, cancellationToken: token);
            await dummy.StrikeAtk();

            while (FSMBoss2EnemySM.minionHave.Count > 0)
            {
                await UniTask.Yield(cancellationToken: token);
            }

            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            dummy.Die();

        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public int RandomPos()
    {
        int select = spawnPoint[Random.Range(0, spawnPoint.Count)];
        spawnPoint.Remove(select);
        return select;
    }

    public async UniTask Attack4()
    {
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        var sound = state.sound;

        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            await dummy.StrikeB2FSM();
            await LaserB2FSM();
            sound.PlayPreAtk(9);
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
            sound.PlayMonAtk(3);
            await UniTask.WaitForSeconds(0.2f, cancellationToken: token);
            await dummy.StrikeAtk();

            for (int i = 0; i < 2; i++)
            {
                await LaserB2FSM();
                sound.PlayPreAtk(9);
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                sound.PlayMonAtk(3);
                await UniTask.WaitForSeconds(0.2f, cancellationToken: token);
                await dummy.StrikeAtk();
            }

            dummy.Die();
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
        var sound = state.sound;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

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
