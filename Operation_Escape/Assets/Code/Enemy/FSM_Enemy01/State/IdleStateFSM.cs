using UnityEngine;

public class IdleStateFSM : BaseState
{
    float time;
    public IdleStateFSM(FSMEnemyM1 stateMachine) : base("Idle", stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        time = 0;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        time += Time.deltaTime;
        Debug.Log(time);
        if (time > 3)
        {
           // stateMachine.ChangState((FSMEnemyM1)stateMachine.wanderState);
        }
    }
}
