using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AreaAttackB2FSM : BaseState
{
    public AreaAttackB2FSM(FSMBoss2EnemySM stateMachine) : base("Burrow", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        ai.canMove = false;
        Attack().Forget();
    }

    public async UniTaskVoid Attack() // Pass the CancellationToken
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            await UniTask.WaitForSeconds(0.5f);
            state.Jump(state.jumpCenter.position);
            await UniTask.WaitForSeconds(0.5f);

            ani.ChangeAnimationAttack("Area_PreAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            bool increasing = true;
            int radius = 5;

            for (int i = 0; i < 3; i++)
            {
                Debug.Log(1);
                for (int j = 0; j < 7; j++)
                {
                    state.SpawnParticle(radius);
                    if (increasing)
                    {
                        radius += 5;
                        if (radius > 35)
                        {
                            radius = 35;
                            increasing = false;
                        }
                    }
                    else
                    {
                        radius -= 5;
                        if (radius < 5)
                        {
                            radius = 5;
                            increasing = true;
                        }
                    }
                    await UniTask.WaitForSeconds(0.5f);
                }
            }

            ani.ChangeAnimationAttack("Area_EndAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
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
