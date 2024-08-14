using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DashAB1FSM : BaseState
{
    public DashAB1FSM(FSMBoss1EnemySM stateMachine) : base("RangeAttack", stateMachine) { }
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
        Debug.Log("Change Mode 1s");
        await UniTask.WaitForSeconds(1f);
        Debug.Log("Chage 1s");
        await UniTask.WaitForSeconds(1f);
        ai.Maxspeed = speed * 2;
        await UniTask.WaitForSeconds(4f);
        if (Vector2.Distance(ai.target.position, ai.position) < 3)
        {
            Debug.Log("Attack 0.5s");
            await UniTask.WaitForSeconds(0.5f);
            Debug.Log("Attack");
            await UniTask.WaitForSeconds(0.5f);

        }
        else
        {

        }
        ai.canMove = true;
        //stateMachine.ChangState(((FSMMiniBossEnemySM)stateMachine).CheckDistance);

    }
}
