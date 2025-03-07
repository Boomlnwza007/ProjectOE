using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CheckNextB2FSM : BaseState
{
    public CheckNextB2FSM(FSMBoss2EnemySM stateMachine) : base("Swarm", stateMachine) { }
    public List<int> rNumber = new List<int> { 1, 2, 3, 4 };
    public List<int> rNumberP2 = new List<int> { 1, 2, 3};

    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        if (state.phase)
        {
            if (rNumberP2.Count == 0)
            {
                rNumberP2 = new List<int> { 1, 2, 3};
            }
            int index = Random.Range(0, rNumberP2.Count);
            int selectedAttack = rNumberP2[index];
            rNumberP2.RemoveAt(index);
            ChangState(CaseStateP2(selectedAttack));
            state.curStateName = state.curState.nameState;
        }
        else
        {
            if (rNumber.Count == 0)
            {
                rNumber = new List<int> { 1, 2, 3, 4 };
            }
            int index = Random.Range(0, rNumber.Count);
            int selectedAttack = rNumber[index];
            rNumber.RemoveAt(index);
            ChangState(CaseState(selectedAttack));
            state.curStateName = state.curState.nameState;
        }
    }

    public BaseState CaseStateP2(int number)
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        switch (number)
        {
            case 1:
                return state.mStrike;
            case 2:
                return state.mArea;
            case 3:
                return state.mSwarm;
            default:
                return state.mStrike;
        }
    }

    public BaseState CaseState(int number)
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        switch (number)
        {
            case 1 :
                return state.strike;
            case 2:
                return state.area;
            case 3:
                return state.swarm;
            case 4:
                return state.laserState;
            default:
                return state.strike;
        }
    }

    public override void Exit()
    {
        // Cancel the attack when exiting the state
    }
}
