
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

    public override void Enter()
    {
        var state = (FSMHeart3EnemySM)stateMachine;
        state.AttackZSpike();
    }

    public override void UpdateLogic()
    {

        if (SpikeZ.hit)
        {
            var state = (FSMHeart3EnemySM)stateMachine;
            Cooldown().Forget();
            ChangState(state.attack);
            return;
        }
        else if (SpikeZ.end)
        {
            var state = (FSMHeart3EnemySM)stateMachine;
            count++;
            if (count > 3)
            {
                count = 0;
                Cooldown().Forget();
                ChangState(state.attack);
            }
            else
            {
                ChangState(state.summon);
            }
            return;
        }

    }

    public async UniTask Cooldown()
    {
        var state = (FSMHeart3EnemySM)stateMachine;
        cooldown = true;
        await UniTask.WaitForSeconds(state.timeCooldownMinion);
        cooldown = false;
    }
}
