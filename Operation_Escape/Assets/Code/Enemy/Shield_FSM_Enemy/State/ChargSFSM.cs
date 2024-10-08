using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChargSFSM : BaseState
{
    public ChargSFSM(FSMSEnemySM stateMachine) : base("Charge", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    float time;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = state.ai;
        speed = ai.maxspeed;
        Debug.Log("µ—Èß∑Ë“‡µ√’¬¡‚®¡µ’");        
        ai.randomDeviation = false;
        time = 0;
        ai.canMove = true;
        if (!state.cooldown)
        {
            Attack().Forget();
        }
        else
        {
            stateMachine.ChangState(state.checkDistanceState);
        }
        //cooldown = false;
    }

    public async UniTask Attack()
    {
        var state = (FSMSEnemySM)stateMachine;
        state.canGuard = true;
        ai.maxspeed = speed * 2;
        while (time < 2f)
        {
            ai.randomDeviation = false;
            time += Time.deltaTime;
            if (Vector2.Distance(ai.targetTransform.position, ai.position) < 2)
            {
                ai.maxspeed = speed;
                state.cooldown = true;
                stateMachine.ChangState(state.bashState);
                return;
            }
            await UniTask.Yield();
        }
        ai.canMove = false;
        await UniTask.WaitForSeconds(2f);
        ai.randomDeviation = true;
        ai.maxspeed = speed;
        stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
    }

    public override void UpdateLogic()
    {     
        //ai.destination = ai.targetTransform.position;

        //if (((FSMSEnemySM)stateMachine).cooldown)
        //{
        //    Debug.Log("µ‘¥ CD");
        //    ((FSMSEnemySM)stateMachine).canGuard = false;
        //    ai.randomDeviation = true;
        //    ai.Maxspeed = speed;
        //    stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
        //    return;
        //}
        //else
        //{
        //    ((FSMSEnemySM)stateMachine).canGuard = true;
        //}

        //distance = Vector2.Distance(ai.position, ai.targetTransform.position);

        //if (cooldown)
        //{
        //    Debug.Log("À¬ÿ¥Õ¬ŸË°—∫∑’Ë 2s");
        //    time += Time.deltaTime;
        //    if (time > 2)
        //    {
        //        ai.canMove = true;
        //        time = 0;
        //        ai.randomDeviation = true;
        //        ai.Maxspeed = speed;
        //        stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
        //    }
        //    return;
        //}

        //if (distance < 2 && !cooldown)
        //{
        //    ai.randomDeviation = true;
        //    ai.Maxspeed = speed;
        //    time += Time.deltaTime;
        //    Debug.Log("µ—Èß∑Ë“‚®¡µ’ 0.5s");
        //    if (time > 0.5f)
        //    {
        //        Debug.Log("‚®¡µ’·∫∫µ‘¥µ“¡");
        //        Debug.Log("∑Ë“‚®¡µ’Approching Attack 1 µ‘¥ CD");
        //        ((FSMSEnemySM)stateMachine).cooldown = true;
        //        ((FSMSEnemySM)stateMachine).CooldownCharg();
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
        //        ai.randomDeviation = true;
        //        ai.Maxspeed = speed;
        //        cooldown = true;
        //    }
        //}
    }
}
