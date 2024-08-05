using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackEMFSM : BaseState
{
    public NormalAttackEMFSM(FSMEnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMEnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        Debug.Log("��駷�����������1 0.5");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;
        time += Time.deltaTime;

        if (time >= 4.8f)
        {
            ai.canMove = true;
            stateMachine.ChangState(((FSMEnemySM)stateMachine).CheckDistance);
        }
        else if (time >= 2.8f)
        {
            Debug.Log("���� 3");
            Debug.Log("��ش����Ѻ��� 2s");
            ai.canMove = false;
        }
        else if (time >= 1.3f)
        {
            Debug.Log("���� 2");
            Debug.Log("��駷������3 1.5s");
        }
        else if (time >= 0.5f)
        {
            Debug.Log("���� 1");
            Debug.Log("��駷������2 0.8s");

        }
    }

}
