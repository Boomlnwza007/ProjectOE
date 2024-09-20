using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderRFSM : BaseState
{
    public WanderRFSM(FSMREnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 5f;
    float time;

    public override void Enter()
    {
        ai = ((FSMREnemySM)stateMachine).ai;
        ai.destination = Randomposition(ai.position, distane);
        time = 0;
        if (((FSMREnemySM)stateMachine).areaEnermy!= null)
        {
            distane = ((FSMREnemySM)stateMachine).areaEnermy.Size();

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

        if (Vector2.Distance(ai.position, ai.targetTransform.position) < ((FSMREnemySM)stateMachine).visRange)
        {
            ((FSMREnemySM)stateMachine).CombatPhaseOn();
            stateMachine.ChangState(((FSMREnemySM)stateMachine).checkDistanceState);
        }

        if (((FSMREnemySM)stateMachine).attacking)
        {
            stateMachine.ChangState(((FSMREnemySM)stateMachine).checkDistanceState);
        }
    }

    public Vector2 Randomposition(Vector2 position, float Size)
    {
        var point = Random.insideUnitCircle * (Size - 2f);
        point += position;
        return point;
    }
}
