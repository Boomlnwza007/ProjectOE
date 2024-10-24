using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NormalAttackRFSM : BaseState
{
    public NormalAttackRFSM(FSMREnemySM stateMachine) : base("NormalAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    //float time;
    //int bulltCount;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMREnemySM)stateMachine).ai;
        speed = ai.maxspeed;
        //time = 0;
        //bulltCount = 0;
        ai.canMove = true;
        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        var state = ((FSMREnemySM)stateMachine);
        state.Movement();
        //time += Time.deltaTime;  
        
        //if (bulltCount >=3)
        //{
        //    if (time >= state.fireCooldown)
        //    {
        //        stateMachine.ChangState(state.checkDistanceState);
        //        time = 0;
        //    }
        //}

        //if (time >= state.fireRate && bulltCount <= 3)
        //{
        //    bulltCount++;
        //    Debug.Log(bulltCount);
        //    state.Fire();
        //    time = 0;
        //}
    }

    public async UniTask Attack()
    {
        var state = ((FSMREnemySM)stateMachine);
        for (int i = 0; i < 3; i++)
        {
            if (Vector2.Distance(ai.destination, ai.position) < 3f)
            {
                stateMachine.ChangState(state.closeAttackState);
                return;
            }            
            await UniTask.WaitForSeconds(state.fireRate);
            ai.canMove = false;
            state.rb.velocity = Vector2.zero;
            await state.PreAttack("PreAttack", 0.1f);
            await state.Attack("Attack", 0.1f);
            state.Fire();
            await UniTask.WaitForSeconds(0.2f); ;
            state.animator.ChangeAnimationAttack("Normal");            
            ai.canMove = true;
        }
        await UniTask.WaitForSeconds(state.fireCooldown);
        stateMachine.ChangState(state.checkDistanceState);
    }
}
