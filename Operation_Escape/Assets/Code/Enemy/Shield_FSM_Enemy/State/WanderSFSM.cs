using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSFSM : BaseState
{
    public WanderSFSM(FSMSEnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 5f;
    //float time;
    public Vector2 center;
    private bool exit;

    public override void Enter()
    {
        ai = ((FSMSEnemySM)stateMachine).ai;
        ai.destination = Randomposition(ai.position, distane);
        exit = false;
        //time = 0;

        if (((FSMSEnemySM)stateMachine).areaEnermy != null)
        {
            distane = ((FSMSEnemySM)stateMachine).areaEnermy.Size();
            center = ((FSMSEnemySM)stateMachine).areaEnermy.gameObject.transform.position;
        }
        else
        {
            center = ai.position;
            distane = 5;
        }
    }

    public override void UpdateLogic()
    {

        if (!exit)
        {
            var state = (FSMSEnemySM)stateMachine;

            if (state.areaEnermy != null && !state.areaEnermy.hasPlayer)
                return;

            if (state.attacking)
            {
                exit = true;
                Awake().Forget();
                return;
            }

            if (Vector2.Distance(ai.position, ai.targetTransform.position) < state.visRange)
            {
                exit = true;
                state.CombatPhaseOn();
                Awake().Forget();
            }
        }
        //if (ai.endMove)
        //{
        //    time += Time.deltaTime;
        //    if (time > 3)
        //    {
        //        time = 0;
        //        ai.destination = Randomposition(center, distane);
        //    }
        //}        
    }

    public Vector2 Randomposition(Vector2 position, float Size)
    {
        var point = Random.insideUnitCircle * (Size - 2f);
        point += position;
        return point;
    }

    public async UniTask Awake()
    {
        await UniTask.WaitForSeconds(1f);
        stateMachine.ChangState(((FSMSEnemySM)stateMachine).checkDistanceState);
    }
}
