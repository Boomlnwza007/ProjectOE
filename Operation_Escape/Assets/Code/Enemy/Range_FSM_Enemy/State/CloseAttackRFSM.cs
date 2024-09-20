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
        go = false;
        time = 0;

        var state = (FSMREnemySM)stateMachine;
        if (state.cooldown)
        {
            stateMachine.ChangState(state.normalAttackState);
            return;
        }

        ai = ((FSMREnemySM)stateMachine).ai;
        //speed = ai.Maxspeed;
        //time = 0;
        //ai.Maxspeed = speed * 10;
        Debug.Log("ตั้งท่าเตรียมโจมตี CloseAttack");        
        if (state.rb != null)
        {
            ai.canMove = false;
            Vector2 knockbackDirection = (ai.position - ai.targetTransform.position).normalized;
            Debug.DrawLine(ai.position, ai.position + (ai.position - ai.targetTransform.position).normalized *10);
            state.rb.AddForce(knockbackDirection * 200, ForceMode2D.Impulse);
            ((FSMREnemySM)stateMachine).FireClose();

            Debug.Log("back");
        }
        //ai.destination = ai.position + (ai.position - ai.targetTarnsform.position).normalized * 5;
    }

    public override void UpdateLogic()  
    {
        if (!go)
        {
            time += Time.deltaTime;
            if (time > 0.5f)
            {
                ((FSMREnemySM)stateMachine).rb.velocity = Vector2.zero;
                ai.canMove = true;
                go = true;
            }
        }
        else
        {
            time += Time.deltaTime;
            if (time > 0.5f)
            {
                ((FSMREnemySM)stateMachine).cooldown = true;
                stateMachine.ChangState(((FSMREnemySM)stateMachine).checkDistanceState);
            }
        }      

    }
}
