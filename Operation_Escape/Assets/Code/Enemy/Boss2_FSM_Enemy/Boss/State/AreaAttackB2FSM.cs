using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AreaAttackB2FSM : BaseState
{
    public AreaAttackB2FSM(FSMBoss2EnemySM stateMachine) : base("AreaAtk", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;
    public bool pass;

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
            ani.ChangeAnimationAttack("UnderGroundUP");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Area_PreAttack");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Area_Attacking");

            bool increasing = true;
            float radius = 4.5f;

            for (int i = 0; i < 3; i++)
            {
                Debug.Log(1);
                for (int j = 0; j < 5; j++)
                {
                    state.SpawnLightning();
                    state.SpawnParticle(radius);
                    if (increasing)
                    {
                        radius += 5;
                        if (radius > 25)
                        {
                            radius = 5;
                            //increasing = false;
                        }
                    }
                    await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                }
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

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
