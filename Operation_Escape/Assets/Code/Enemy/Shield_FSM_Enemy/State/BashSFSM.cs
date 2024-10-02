using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BashSFSM : BaseState
{
    public BashSFSM(FSMSEnemySM stateMachine) : base("BashAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = state.ai;
        state.canGuard = true;
        ai.destination = ai.targetTransform.position;
        speed = ai.Maxspeed;
        Attack().Forget();
    }

    public async UniTask Attack()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai.Maxspeed = speed;
        await UniTask.WaitForSeconds(0.5f);
        state.Attack();
        state.canGuard = false;
        ai.canMove = false;
        await UniTask.WaitForSeconds(2f);
        ai.canMove = true;
        stateMachine.ChangState(state.checkDistanceState);
    }

    public override void UpdateLogic()
    {
        //if (cooldown)
        //{
        //    Debug.Log("หยุดอยู่กับที่ 2s");
        //    time += Time.deltaTime;
        //    if (time > 2)
        //    {
        //        ai.canMove = true;
        //        time = 0;
        //        ai.Maxspeed = speed;
        //        stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
        //    }
        //    return;
        //}

        //if (distance < 2.5f && !cooldown)
        //{
        //    ai.Maxspeed = speed;
        //    time += Time.deltaTime;
        //    Debug.Log("ตั้งท่าโจมตี 0.5s");
        //    if (time > 0.5f)
        //    {
        //        Debug.Log("BashAttack");
        //        ai.canMove = false;
        //        cooldown = true;
        //    }
        //}
        //else
        //{
        //    time += Time.deltaTime;
        //    if (time > 2)
        //    {
        //        ai.canMove = false;
        //        time = 0;
        //        ai.Maxspeed = speed;
        //        cooldown = true;
        //    }
        //}
    }
}
