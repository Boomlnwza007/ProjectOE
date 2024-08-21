using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackRFSM : BaseState
{
    public CloseAttackRFSM(FSMREnemySM stateMachine) : base("CloseAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public bool go;
    float time;

    // Start is called before the first frame update
    public override void Enter()
    {
        if (((FSMREnemySM)stateMachine).cooldown)
        {
            stateMachine.ChangState(((FSMREnemySM)stateMachine).normalAttackState);
            return;
        }
        ai = ((FSMREnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        time = 0;
        ai.Maxspeed = speed * 10;
        Debug.Log("ตั้งท่าเตรียมโจมตี CloseAttack");
        ai.destination = ai.position + (ai.position - ai.targetTarnsform.position).normalized * 5;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (ai.endMove)
        {
            go = true;
        }

        if (go)
        {
            ai.Maxspeed = speed;
            time += Time.deltaTime;
            if (time > 0.5f)
            {
                ai.canMove = true;
                ((FSMREnemySM)stateMachine).FireClose();
                ((FSMREnemySM)stateMachine).cooldown = true;
                stateMachine.ChangState(((FSMREnemySM)stateMachine).checkDistanceState);
            }
        }      

    }
}
