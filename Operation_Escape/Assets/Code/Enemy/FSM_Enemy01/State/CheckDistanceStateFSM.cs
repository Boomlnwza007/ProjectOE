using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceStateFSM : BaseState
{
    public IAstarAI ai;
    public Transform target;
    public float distance;
    public CheckDistanceStateFSM(FSMEnemyM1 stateMachine) : base("CheckDistance", stateMachine) { }
    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
        GetData();
        ai.canMove = true;
        ai.destination = target.position;
        distance = Vector2.Distance(ai.position, target.position);
        if (distance < 2)
        {
            stateMachine.ChangState(((FSMEnemyM1)stateMachine).N1Attack);
        }
        else
        {
            stateMachine.ChangState(((FSMEnemyM1)stateMachine).Charge);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = target.position;
    }

    public void GetData()
    {
        ai = (IAstarAI)stateMachine.Getdata("ai");
        target = (Transform)stateMachine.Getdata("target");
    }
}
