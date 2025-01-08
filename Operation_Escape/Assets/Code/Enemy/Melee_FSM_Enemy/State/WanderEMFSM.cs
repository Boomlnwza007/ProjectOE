using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class WanderEMFSM : BaseState
{
    public WanderEMFSM(FSMMEnemySM stateMachine) : base("Wander", stateMachine) { }
    public IAiAvoid ai;
    public float distane = 5f;
    //float time;
    public Vector2 center;
    private bool exit;

    public override void Enter()
    {
        ai = ((FSMMEnemySM)stateMachine).ai;
        ai.destination = Randomposition(ai.position, distane);
        ai.canMove = true;
        exit = false;
        //time = 0;

        if (((FSMMEnemySM)stateMachine).areaEnermy != null)
        {
            distane = ((FSMMEnemySM)stateMachine).areaEnermy.Size();
            center = ((FSMMEnemySM)stateMachine).areaEnermy.gameObject.transform.position;
        }
        else
        {
            distane = 7;
            center = ai.position;
        }
        ai.randomDeviation = false;
    }

    public override void UpdateLogic()
    {
        if (!exit)
        {
            var enemySM = (FSMMEnemySM)stateMachine;

            if (enemySM.areaEnermy != null && !enemySM.areaEnermy.hasPlayer)
                return;

            if (enemySM.attacking)
            {
                exit = true;
                Awake().Forget();
                return;
            }

            if (Vector2.Distance(ai.position, ai.targetTransform.position) < enemySM.visRange)
            {
                exit = true;
                enemySM.CombatPhaseOn();
                Awake().Forget();
            }
        }

        //if (ai.endMove)
        //{
        //    time += Time.deltaTime;
        //    if (time > 3)W
        //    {
        //        time = 0;
        //        ai.destination = Randomposition(ai.position, distane);
        //    }
        //}


    }

    public Vector2 Randomposition(Vector2 position, float Size)
    {
        var point = Random.insideUnitCircle * (Size-2f);
        point += position;
        return point;        
    }

    public async UniTask Awake()
    {
        await UniTask.WaitForSeconds(1f);
        stateMachine.ChangState(((FSMMEnemySM)stateMachine).CheckDistance);
    }
}
