using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargSFSM : BaseState
{
    public ChargSFSM(FSMSEnemySM stateMachine) : base("Charge", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;
    float distance;
    bool cooldown;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMSEnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        Debug.Log("µÑé§·èÒàµÃÕÂÁâ¨ÁµÕ");
        ai.Maxspeed = speed * 2;
        cooldown = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;

        if (((FSMSEnemySM)stateMachine).cooldown)
        {
            Debug.Log("µÔ´ CD");
            ((FSMSEnemySM)stateMachine).canGuard = false;
            ai.Maxspeed = speed;
            stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
            return;
        }
        else
        {
            ((FSMSEnemySM)stateMachine).canGuard = true;
        }

        distance = Vector2.Distance(ai.position, ai.target.position);

        if (cooldown)
        {
            Debug.Log("ËÂØ´ÍÂÙè¡Ñº·Õè 2s");
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

        if (distance < 2 && !cooldown)
        {
            ai.Maxspeed = speed;
            time += Time.deltaTime;
            Debug.Log("µÑé§·èÒâ¨ÁµÕ 0.5s");
            if (time > 0.5f)
            {
                Debug.Log("â¨ÁµÕáººµÔ´µÒÁ");
                Debug.Log("·èÒâ¨ÁµÕApproching Attack 1 µÔ´ CD");
                ((FSMSEnemySM)stateMachine).cooldown = true;
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
