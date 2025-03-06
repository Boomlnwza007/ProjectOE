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
    private bool final;
    public List<int> rNumber = new List<int> { 2, 3, 4 };

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        Attack().Forget();
        ai.canMove = false;
        if (rNumber.Count == 0)
        {
            rNumber = new List<int> { 2, 3, 4 };
        }
        int index = Random.Range(0, rNumber.Count);
        int selectedAttack = rNumber[index];
        rNumber.RemoveAt(index);

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
            await UniTask.WaitForSeconds(1f, cancellationToken: token);

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
