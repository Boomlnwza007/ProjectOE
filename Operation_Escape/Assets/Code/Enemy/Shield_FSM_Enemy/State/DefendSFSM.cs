using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cysharp.Threading.Tasks;
//using System.Threading;
//using System;

public class DefendSFSM : BaseState
{
    public DefendSFSM(FSMSEnemySM stateMachine) : base("DefendlAttack", stateMachine) { }
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
