using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NoShieldSFSM : BaseState
{
    public NoShieldSFSM(FSMSEnemySM stateMachine) : base("NoShieldState", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    public FSMSEnemySM nearMon;
    public bool cooldownBuff;
    public bool attack;

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMSEnemySM)stateMachine;
        ai = state.ai;
        speed = ai.maxspeed;
        ai.randomDeviation = false;
        ai.canMove = true;
        nearMon = null;
    }

    public override void UpdateLogic()
    {
        var state = (FSMSEnemySM)stateMachine;
        //if (attack)
        //{
        //    state.Movement();
        //}

        if (state.shield.canGuard)
        {
            ChangState(state.checkDistanceState);
        }

        if (!cooldownBuff)
        {
            if (Check())
            {
                buff().Forget();
            }            
        }

        if (!Check() || cooldownBuff)
        {
            if (Vector2.Distance(ai.position,ai.targetTransform.position) < 5)
            {
                if (!attack)
                {
                    Attack().Forget();
                }
            }
        }
    }

    public bool Check()
    {
        var state = (FSMSEnemySM)stateMachine;
        //foreach (StateMachine mon in state.areaEnermy.enemy)
        //{
        //    if (mon is FSMSEnemySM enemyState)
        //    {
        //        if (Vector2.Distance(mon.gameObject.transform.position,ai.position)<10)
        //        {
        //            nearMon = enemyState;
        //            return true;
        //        }
        //    }
        //}

        return false;
    }

    public async UniTaskVoid buff()
    {
        cooldownBuff = true;
        await UniTask.WaitForSeconds(3);
        nearMon.shield.canGuard = true;
        await UniTask.WaitForSeconds(6);
        cooldownBuff = false;
    }

    public async UniTaskVoid Attack()
    {
        attack = true;
        await UniTask.WaitForSeconds(0.5f);
        Debug.Log("attack");
        await UniTask.WaitForSeconds(1f);
        attack = true;
    }
}
