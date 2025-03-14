using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StrikeB2FSM : BaseState
{
    public StrikeB2FSM(FSMBoss2EnemySM stateMachine) : base("Strike", stateMachine) { }
    public IAiAvoid ai;
    private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss2EnemySM)stateMachine).ai;
        Attack().Forget();
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        var sound = state.sound;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            ai.canMove = false;
            sound.PlayPreAtk(1);
            ani.ChangeAnimationAttack("Strike");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            await Strike();
            RandomEdge();
            for (int i = 0; i < 3; i++)
            {
                await UniTask.WaitForSeconds(2);
                await Strike();
                RandomEdge();
            }

            state.colliderBoss.isTrigger = false;
            ani.ChangeAnimationAttack("UnderGroundUP");
            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            ani.ChangeAnimationAttack("Wait");
            ChangState(state.eat);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTask Strike()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        var sound = state.sound;

        state.colliderBoss.isTrigger = true;
        Vector2 dir = Diraction();

        sound.PlayMonAtk(1);
        state.rb.velocity = dir * state.speedStrike;

        while (!state.inRoom)
        {
            dir = Diraction();
            state.rb.velocity = dir * state.speedStrike;
            await UniTask.Yield(cancellationToken: cancellationToken.Token);
        }

        while (state.inRoom)
        {
            state.rb.velocity = dir * state.speedStrike;
            await UniTask.Yield(cancellationToken: cancellationToken.Token);
        }            
    }

    public Vector2 Diraction()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        var ani = state.animator;
        Vector2 dir = (ai.targetTransform.position - ai.position).normalized;        
        if (dir.x < 0)
        {
            ani.ChangeAnimationAttack("Striking_L");
        }
        else
        {
            ani.ChangeAnimationAttack("Striking_R");
        }
        return dir;
    }

    public void RandomEdge()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        int rEdge = Random.Range(0, 4);
        int rNumber = 0;
        state.rb.velocity = Vector2.zero;
        Vector3 pos;
        switch (rEdge)
        {
            case 0 :
                rNumber = Random.Range(0, state.areaMark.top.Length);
                pos = state.areaMark.top[rNumber].position;
                pos.x += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
            case 1:
                rNumber = Random.Range(0, state.areaMark.down.Length);
                pos = state.areaMark.down[rNumber].position;
                pos.x += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
            case 2:
                rNumber = Random.Range(0, state.areaMark.left.Length);
                pos = state.areaMark.left[rNumber].position;
                pos.y += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
            case 3:
                rNumber = Random.Range(0, state.areaMark.right.Length);
                pos = state.areaMark.left[rNumber].position;
                pos.y += Random.Range(-5, 5);
                state.transform.position = pos;

                break;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
        cancellationToken?.Cancel();
    }
}
