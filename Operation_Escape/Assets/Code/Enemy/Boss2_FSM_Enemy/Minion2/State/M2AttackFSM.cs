using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M2AttackFSM : BaseState
{
    public M2AttackFSM(FSMMinion2EnemySM stateMachine) : base("Attacck", stateMachine) { }
    public IAiAvoid ai;
    private float timer;
    private float time;
    private bool shoot;
    // Start is called before the first frame update
    public override void Enter()
    {
        var state = ((FSMMinion2EnemySM)stateMachine);
        time = Random.Range(state.minShoot, state.minShoot);
        timer = 0;
    }

    public override void UpdateLogic()
    {
        timer += Time.deltaTime;
        if (timer >= time)
        {
            var state = ((FSMMinion2EnemySM)stateMachine);
            if (!shoot)
            {
                shoot = true;
                state.animator.SetTrigger("Shoot");
            }
            if (timer >= time + 0.5f)
            {
                state.Fire();
                timer = 0;
                time = Random.Range(state.minShoot, state.maxShoot);
                state.animator.SetTrigger("Shoot");
                shoot = false;
            }
        }
    }
}
