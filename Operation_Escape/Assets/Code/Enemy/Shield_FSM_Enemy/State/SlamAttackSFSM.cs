using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class SlamAttackSFSM : BaseState
{
    public SlamAttackSFSM(FSMSEnemySM stateMachine) : base("slamAttState", stateMachine) { }
    private CancellationTokenSource cancellationToken;
    public IAiAvoid ai;
    public float speed;
    public bool cooldown;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = state.ai;
        ai.destination = ai.targetTransform.position;
        speed = ai.maxspeed;
        if (!cooldown)
        {
            Attack().Forget();
        }
        else
        {
            ChangState(state.checkDistanceState);
        }
    }

    public async UniTask Attack()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = (FSMSEnemySM)stateMachine;
        var ani = state.animator;

        try
        {
            await UniTask.WaitForSeconds(1);
            Debug.Log("attack");
            cooldown = true;
            Cooldown().Forget();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTaskVoid Cooldown()
    {
        await UniTask.WaitForSeconds(2);
        cooldown = false;
    }
}
