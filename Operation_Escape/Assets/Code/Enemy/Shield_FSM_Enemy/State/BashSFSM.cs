using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashSFSM : BaseState
{
    public BashSFSM(FSMSEnemySM stateMachine) : base("BashAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;
    float distance;
    bool cooldown;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMSEnemySM)stateMachine).ai;
        ((FSMSEnemySM)stateMachine).canGuard = false;
        ai.destination = ai.targetTransform.position;
        speed = ai.Maxspeed;
    }

    public override void UpdateLogic()
    {
        if (cooldown)
        {
            Debug.Log("หยุดอยู่กับที่ 2s");
            time += Time.deltaTime;
            if (time > 2)
            {
                ai.canMove = true;
                time = 0;
                ai.Maxspeed = speed;
                stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
            }
            return;
        }

        if (distance < 2.5f && !cooldown)
        {
            ai.Maxspeed = speed;
            time += Time.deltaTime;
            Debug.Log("ตั้งท่าโจมตี 0.5s");
            if (time > 0.5f)
            {
                Debug.Log("BashAttack");
                ai.canMove = false;
                cooldown = true;
            }
        }
        else
        {
            time += Time.deltaTime;
            if (time > 2)
            {
                ai.canMove = false;
                time = 0;
                ai.Maxspeed = speed;
                cooldown = true;
            }
        }
    }
}
