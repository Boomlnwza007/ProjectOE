using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DashAB1FSM : BaseState
{
    public DashAB1FSM(FSMBoss1EnemySM stateMachine) : base("DashAttack", stateMachine) { }
    public IAiAvoid ai;
    public float speed;
    private bool followMeLee;
    public bool overdrive;

    // Start is called before the first frame update
    public override void Enter()
    {
        ai = ((FSMBoss1EnemySM)stateMachine).ai;
        overdrive = ((FSMBoss1EnemySM)stateMachine).overdrive;
        speed = ai.Maxspeed;
        ai.canMove = true;
        Attack().Forget();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        ai.destination = ai.target.position;

        if (followMeLee)
        {
            ((FSMBoss1EnemySM)stateMachine).MeleeFollow();
        }
    }

    public async UniTask Attack()
    {
        Debug.Log("Change Mode 1s");
        await UniTask.WaitForSeconds(1f);
        Debug.Log("Chage 1s");
        await UniTask.WaitForSeconds(1f);
        ai.Maxspeed = speed * 2;
        float time = 0;
        while (time < 4f)
        {
            time += Time.deltaTime;
            if (Vector2.Distance(ai.target.position, ai.position) < 3)
            {
                ai.Maxspeed = speed;
                await UniTask.WhenAll(((FSMBoss1EnemySM)stateMachine).MeleeHitzone(0.5f, 1), Aim(0.4f));
                ai.canMove = false;
                await UniTask.WaitForSeconds(1);
                ai.canMove = true;
                stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).normalAState);
                return;
            }
            await UniTask.Yield();
        }
       ai.Maxspeed = speed;
       stateMachine.ChangState(((FSMBoss1EnemySM)stateMachine).normalAState);
    }

    public async UniTask Aim(float wait)
    {
        followMeLee = true;
        await UniTask.WaitForSeconds(wait);
        followMeLee = false;
    }
}
