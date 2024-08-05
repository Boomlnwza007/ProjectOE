using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderEMFSM : BaseState
{
    public WanderEMFSM(FSMEnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 15f;
    float time;

    public override void Enter()
    {
        ai = ((FSMEnemySM)stateMachine).ai;
        ai.destination = Randomposition(ai.position, distane);
        time = 0;
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

        if (Vector2.Distance(ai.position, ai.target.position) < ((FSMEnemySM)stateMachine).visRange)
        {
            ((FSMEnemySM)stateMachine).areaEnermy.combatPhase();
            stateMachine.ChangState(((FSMEnemySM)stateMachine).CheckDistance);
        }
    }

    public Vector3 Randomposition(Vector3 position, float Size)
    {
        var point = Random.insideUnitSphere * Size;
        point.z = 0;
        point += position;
        return point;        
    }
}
