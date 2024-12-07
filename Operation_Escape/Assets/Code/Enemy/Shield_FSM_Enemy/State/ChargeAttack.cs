using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cysharp.Threading.Tasks;
//using System.Threading;
//using System;

public class ChargeAttack : BaseState
{
    public ChargeAttack(FSMSEnemySM stateMachine) : base("chargeAttState", stateMachine) { }
    public IAiAvoid ai;
    //private CancellationTokenSource cancellationToken;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.destination = ai.targetTransform.position;

        ChangState(state.checkDistanceState);
    }

    public override void UpdateLogic()
    {

    }

    //public async UniTask Attack()
    //{
    //    cancellationToken = new CancellationTokenSource();
    //    var token = cancellationToken.Token;
    //    var state = (FSMSEnemySM)stateMachine;
    //    var ani = state.animator;

    //    try
    //    {

    //    }
    //    catch (OperationCanceledException)
    //    {
    //        Debug.Log("Attack was cancelled.");
    //        return;
    //    }
    //}

}
