using Pathfinding;
using UnityEngine;

public class WanderStateFSM : BaseState
{
    public IAstarAI ai;
    public Transform target;
    float time;
    public WanderStateFSM(FSMEnemyM1 stateMachine) : base("Wander", stateMachine) { }
    public float distane = 15f;

    public override void Enter()
    {
        base.Enter();
        GetData();
        ai.destination = Randomposition(ai.position, distane);
        ai.SearchPath();
        time = 0;        
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            time += Time.deltaTime;
            if (time > 3)
            {
                ai.destination = Randomposition(ai.position, distane);
                ai.SearchPath();
            }
        }

        if (Vector2.Distance(ai.position, target.position) < 10)
        {           
            stateMachine.ChangState(((FSMEnemyM1)stateMachine).CheckDistance);
        }
    }

    public Vector3 Randomposition(Vector3 position, float Size)
    {
        var point = Random.insideUnitSphere * Size;
        point.z = 0;
        point += position;
        return point;
    }

    public void GetData()
    {
        ai = (IAstarAI)stateMachine.Getdata("ai");
        target = (Transform)stateMachine.Getdata("target");
    }

}
