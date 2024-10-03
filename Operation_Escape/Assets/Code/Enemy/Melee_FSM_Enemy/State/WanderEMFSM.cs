using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderEMFSM : BaseState
{
    public WanderEMFSM(FSMMEnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 5f;
    float time;

    public override void Enter()
    {
        ai = ((FSMMEnemySM)stateMachine).ai;
        ai.destination = Randomposition(ai.position, distane);
        ai.canMove = true;
        time = 0;
        if (((FSMMEnemySM)stateMachine).areaEnermy != null)
        {
            distane = ((FSMMEnemySM)stateMachine).areaEnermy.Size();
        }
        else
        {
            distane = 7;
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

        if (((FSMMEnemySM)stateMachine).areaEnermy != null)
        {
            if (!((FSMMEnemySM)stateMachine).areaEnermy.hasPlayer)
            {
                return;
            }
        }

        if (Vector2.Distance(ai.position, ai.targetTransform.position) < ((FSMMEnemySM)stateMachine).visRange)
        {
            ((FSMMEnemySM)stateMachine).CombatPhaseOn();
            stateMachine.ChangState(((FSMMEnemySM)stateMachine).CheckDistance);
        }

        if (((FSMMEnemySM)stateMachine).attacking)
        {
            stateMachine.ChangState(((FSMMEnemySM)stateMachine).CheckDistance);
        }
    }

    public Vector2 Randomposition(Vector2 position, float Size)
    {
        var point = Random.insideUnitCircle * (Size-2f);
        point += position;
        return point;        
    }
}
