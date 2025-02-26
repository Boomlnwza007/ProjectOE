
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class H3SummonFSM : BaseState
{
    public H3SummonFSM(FSMHeart3EnemySM stateEnemy) : base("Summon", stateEnemy) { }
    private CancellationTokenSource cancellationToken;
    public bool cooldown;
    public int count = 0;
    private bool endState;

    public override void Enter()
    {
        var state = (FSMHeart3EnemySM)stateMachine;
        state.AttackZSpike();
        endState = false;
        state.shield.ShieldIsOn(false);
    }

    public override void UpdateLogic()
    {
        if (!endState)
        {
            if (SpikeZ.end)
            {
                if (SpikeZ.hit || ++count > 3)
                {
                    endState = true;
                    ExitState().Forget();
                }
                else
                {
                    var state = (FSMHeart3EnemySM)stateMachine;
                    ChangState(state.summon);
                }
            }
        }     

    }

    public async UniTask ExitState()
    {
        await UniTask.WaitForSeconds(0.5f);
        var state = (FSMHeart3EnemySM)stateMachine;
        count = 0;
        Cooldown().Forget();
        state.shield.ShieldIsOn(true);
        ChangState(state.attack);
    }

    public async UniTask Cooldown()
    {
        var state = (FSMHeart3EnemySM)stateMachine;
        cooldown = true;
        await UniTask.WaitForSeconds(state.timeCooldownMinion);
        cooldown = false;
    }
}
