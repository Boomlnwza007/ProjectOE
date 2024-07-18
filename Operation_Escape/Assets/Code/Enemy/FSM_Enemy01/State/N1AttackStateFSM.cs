using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N1AttackStateFSM : BaseState
{
    public IAstarAI ai;
    public Transform target;
    public float speed;
    float time;
    public N1AttackStateFSM(FSMEnemyM1 stateMachine) : base("N1Atack", stateMachine) { }
    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
        GetData();
        ai.destination = target.position;
        ai.canMove = false;
        time = 0;
        Debug.Log("N1");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = target.position;
        time += Time.deltaTime;
        Vector3 rotation = ai.position - target.position;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        ai.rotation = Quaternion.Euler(0, 0, rot+90);
        if (time > 2)
        {
            ai.canMove = true;
            stateMachine.ChangState(((FSMEnemyM1)stateMachine).CheckDistance);
        }

    }

    public void GetData()
    {
        ai = (IAstarAI)stateMachine.Getdata("ai");
        target = (Transform)stateMachine.Getdata("target");
        speed = ((FSMEnemyM1)stateMachine).Speed;
    }
}
