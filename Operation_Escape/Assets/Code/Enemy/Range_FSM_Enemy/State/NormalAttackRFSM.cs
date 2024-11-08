using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class NormalAttackRFSM : BaseState
{
    public NormalAttackRFSM(FSMREnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    private CancellationTokenSource cancellationToken;
    //float time;
    //int bulltCount;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMREnemySM)stateMachine).ai;
        speed = ai.maxspeed;
        cancellationToken = new CancellationTokenSource();
        //time = 0;
        //bulltCount = 0;
        ai.canMove = true;
        Attack(cancellationToken.Token).Forget();
    }

    public override void UpdateLogic()
    {
        var state = ((FSMREnemySM)stateMachine);
        state.Movement();        
    }

    public async UniTask Attack(CancellationToken token)
    {
        var state = ((FSMREnemySM)stateMachine);
        try
        {
            for (int i = 0; i < 3; i++)
            {
                if (Vector2.Distance(ai.destination, ai.position) < 3f)
                {
                    stateMachine.ChangState(state.closeAttackState);
                    return;
                }
                await UniTask.WaitForSeconds(state.fireRate, cancellationToken: token);
                ai.canMove = false;
                state.rb.velocity = Vector2.zero;
                await state.PreAttack("PreAttack", 0.1f);
                await state.Attack("Attack", 0.1f);
                state.Fire();
                await UniTask.WaitForSeconds(0.2f, cancellationToken: token); ;
                state.animator.ChangeAnimationAttack("Normal");
                ai.canMove = true;
            }
            await UniTask.WaitForSeconds(state.fireCooldown, cancellationToken: token);
            stateMachine.ChangState(state.checkDistanceState);
        }
        catch (OperationCanceledException)
        {
            return;
        }
       
    }
}
