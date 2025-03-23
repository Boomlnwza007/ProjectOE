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
    private int bulltCount = 0;
    private bool firing;


    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMREnemySM)stateMachine).ai;
        speed = ai.maxspeed;
        cancellationToken = new CancellationTokenSource();
        firing = false;
        ai.canMove = true;
    }

    public override void UpdateLogic()
    {
        var state = ((FSMREnemySM)stateMachine);
        state.Movement();

        if (Vector2.Distance(ai.destination, ai.position) < 3f && bulltCount < 3)
        {
            ChangState(state.closeAttackState);
            return;
        }

        if (Fire() && !firing)
        {
            firing = true;
            Attack(cancellationToken.Token).Forget();
            return;
        }        
    }

    public async UniTask Attack(CancellationToken token)
    {
        var state = ((FSMREnemySM)stateMachine);
        try
        {
            if (bulltCount < 3)
            {
                bulltCount++;
                ai.canMove = false;
                state.rb.velocity = Vector2.zero;
                await state.PreAttack("PreAttack", 0.1f);
                await state.Attack("Attack", 0.1f);
                state.Fire();
                await UniTask.WaitForSeconds(0.2f, cancellationToken: token); ;
                state.animator.ChangeAnimationAttack("Normal");
                ai.canMove = true;
                await UniTask.WaitForSeconds(state.fireRate, cancellationToken: token);
            }
            else
            {
                await UniTask.WaitForSeconds(state.fireCooldown, cancellationToken: token);
                bulltCount = 0;
                ChangState(state.checkDistanceState);
                return;
            }    

            firing = false;
        }
        catch (OperationCanceledException)
        {
            return;
        }

    }

    public bool Fire()
    {
        var state = ((FSMREnemySM)stateMachine);
        Vector2 dir = ai.targetTransform.position - ai.position;
        RaycastHit2D hit = Physics2D.Raycast(ai.position, dir, 15, state.raycastMaskFire);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }
}
