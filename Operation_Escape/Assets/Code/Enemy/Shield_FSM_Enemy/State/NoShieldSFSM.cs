using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NoShieldSFSM : BaseState
{
    public NoShieldSFSM(FSMSEnemySM stateMachine) : base("NoShieldState", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public FSMSEnemySM nearMon;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = state.ai;
        speed = ai.maxspeed;
        ai.randomDeviation = false;
        ai.canMove = true;
        if (state.cooldown)
        {

        }
    }

    public override void UpdateLogic()
    {
        var state = (FSMSEnemySM)stateMachine;
        state.Movement();
        if (state.shield.canGuard)
        {
            ChangState(state.checkDistanceState);
        }

        if (nearMon = null)
        {
            Check();
        }
        else
        {

        }
        
    }

    public void Check()
    {
        var state = (FSMSEnemySM)stateMachine;
        foreach (StateMachine mon in state.areaEnermy.enemy)
        {
            if (mon is FSMSEnemySM enemyState)
            {
                if (Vector2.Distance(mon.gameObject.transform.position,ai.position)<10)
                {
                    nearMon = enemyState;
                    return;
                }
            }
        }
    }
}
