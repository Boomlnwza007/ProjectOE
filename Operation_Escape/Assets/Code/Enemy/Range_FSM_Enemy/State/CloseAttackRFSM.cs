using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackRFSM : BaseState
{
    public CloseAttackRFSM(FSMREnemySM stateMachine) : base("CloseAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMREnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        time = 0;
        ai.Maxspeed = speed * 10;
        Debug.Log("ตั้งท่าเตรียมโจมตี CloseAttack");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Debug.DrawLine(ai.position, ai.destination);
        ai.destination = ai.position + (ai.position - ai.target.position).normalized*2;
        time += Time.deltaTime;
        if (time > 0.5)
        {
            ai.destination = ai.target.position;
            ai.canMove = true;
            ai.Maxspeed = speed;
            if (!((FSMMEnemySM)stateMachine).cooldown)
            {
                ((FSMREnemySM)stateMachine).FireClose();
            }
            stateMachine.ChangState(((FSMREnemySM)stateMachine).checkDistanceState);
            ((FSMMEnemySM)stateMachine).cooldown = true;
        }

    }
}
