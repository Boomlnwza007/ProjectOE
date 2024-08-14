using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NormalAB1FSM : BaseState
{
    public NormalAB1FSM(FSMBoss1EnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        speed = ai.Maxspeed;
        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;
    }

    public async UniTask Attack()
    {
        Debug.Log("��駷�����������1 0.5");
        await UniTask.WaitForSeconds(0.5f);
        Debug.Log("���� 1");
        Debug.Log("��駷������2 0.8s");
        await UniTask.WaitForSeconds(0.8f);
        Debug.Log("���� 2");
        Debug.Log("��駷������3 1.5s");
        await UniTask.WaitForSeconds(1.5f);
        ai.canMove = false;
        Debug.Log("���� 3");
        Debug.Log("��ش����Ѻ��� 2s");
        await UniTask.WaitForSeconds(2f);
        ai.canMove = true;
        //stateMachine.ChangState(((FSMMiniBossEnemySM)stateMachine).CheckDistance);

    }
}
