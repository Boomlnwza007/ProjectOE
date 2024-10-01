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
        var state = ((FSMREnemySM)stateMachine);
        state.Movement();
        time += Time.deltaTime;  
        
        if (bulltCount >=3)
        {
            if (time >= state.fireCooldown)
            {
                stateMachine.ChangState(state.checkDistanceState);
                time = 0;
            }
        }

        if (time >= state.fireRate)
        {
            bulltCount++;
            Debug.Log(bulltCount);
            state.Fire();
            time = 0;
        }
    }
}
