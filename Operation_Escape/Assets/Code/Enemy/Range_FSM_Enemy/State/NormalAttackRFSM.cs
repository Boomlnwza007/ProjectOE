using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackRFSM : BaseState
{
    public NormalAttackRFSM(FSMREnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;
    int bulltCount;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMREnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        Debug.Log("ตั้งท่าเตรียมโจมตี1 0.5");
        time = 0;
        bulltCount = 0;
        ai.canMove = true;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ((FSMREnemySM)stateMachine).Movement();
        time += Time.deltaTime;  
        
        if (bulltCount >=3)
        {
            stateMachine.ChangState(((FSMREnemySM)stateMachine).checkDistanceState);
        }

        if (time >= 0.8f)
        {
            bulltCount++;
            Debug.Log(bulltCount);
            ((FSMREnemySM)stateMachine).Fire();
            time = 0;
        }
    }
}
