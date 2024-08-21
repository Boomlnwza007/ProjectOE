using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMNBFSM : BaseState
{
    public ChargeMNBFSM(FSMMiniBossEnemySM stateMachine) : base("Charge", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;
    float distance;
    bool cooldown;

    public override void Enter()
    {
        ai = ((FSMMiniBossEnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        Debug.Log("��駷�����������");
        ai.randomDeviation = false;
        ai.Maxspeed = speed * 2;
        cooldown = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.targetTarnsform.position;
        if (((FSMMiniBossEnemySM)stateMachine).cooldown)
        {
            Debug.Log("�Դ CD");
            ai.Maxspeed = speed;
            ai.randomDeviation = true;
            //stateMachine.ChangState(((FSMMiniBossEnemySM)stateMachine).CheckDistance);
            return;
        }

        distance = Vector2.Distance(ai.position, ai.targetTarnsform.position);

        if (cooldown)
        {
            Debug.Log("��ش����Ѻ��� 2s");
            time += Time.deltaTime;
            if (time > 2)
            {
                ai.canMove = true;
                time = 0;
                ai.randomDeviation = true;
                ai.Maxspeed = speed;
                //stateMachine.ChangState(((FSMMiniBossEnemySM)stateMachine).CheckDistance);
            }
            return;
        }

        if (distance < 2 && !cooldown)
        {
            ai.Maxspeed = speed;
            ai.randomDeviation = true;
            time += Time.deltaTime;
            Debug.Log("��駷������ 0.5s");
            if (time > 0.5f)
            {
                Debug.Log("����Ẻ�Դ���");
                Debug.Log("�������Approching Attack 1 �Դ CD");
                ((FSMMiniBossEnemySM)stateMachine).cooldown = true;
                //((FSMMiniBossEnemySM)stateMachine).CooldownApproching();
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
                ai.randomDeviation = true;
                ai.Maxspeed = speed;
                cooldown = true;
            }
        }
    }
}
