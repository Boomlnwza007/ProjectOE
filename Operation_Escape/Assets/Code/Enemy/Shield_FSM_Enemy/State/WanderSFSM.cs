using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSFSM : BaseState
{
    public WanderSFSM(FSMSEnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 5f;
    float time;

    public override void Enter()
    {
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.destination = Randomposition(ai.position, distane);
        time = 0;

        if (((FSMSEnemySM)stateMachine).areaEnermy != null)
        {
            distane = ((FSMSEnemySM)stateMachine).areaEnermy.Size();
        }
        else
        {
            distane = 5;
        }
    }

    public override void UpdateLogic()
    {
        if (ai.endMove)
        {
            time += Time.deltaTime;
            if (time > 3)
            {
                time = 0;
                ai.destination = Randomposition(ai.position, distane);
            }
        }

        if (((FSMSEnemySM)stateMachine).areaEnermy != null)
        {
            if (!((FSMSEnemySM)stateMachine).areaEnermy.hasPlayer)
            {
                return;
            }
        }

        if (Vector2.Distance(ai.position, ai.targetTransform.position) < ((FSMSEnemySM)stateMachine).visRange)
        {
            ((FSMSEnemySM)stateMachine).CombatPhaseOn();
            stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
        }

        if (((FSMSEnemySM)stateMachine).attacking)
        {
            stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
        }
    }

    public Vector2 Randomposition(Vector2 position, float Size)
    {
        var point = Random.insideUnitCircle * (Size - 2f);
        point += position;
        return point;
    }
}
