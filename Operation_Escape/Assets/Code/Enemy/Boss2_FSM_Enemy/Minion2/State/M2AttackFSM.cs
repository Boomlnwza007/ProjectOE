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

    // Start is called before the first frame update
    public override void Enter()
    {
        time = Random.Range(1, 4);
        timer = 0;
    }

    public override void UpdateLogic()
    {
        timer += Time.deltaTime;
        if (timer >= time)
        {
            ((FSMMinion2EnemySM)stateMachine).Fire();
            timer = 0;
            time = Random.Range(1, 4);
            Debug.Log(time);
        }
    }
}
