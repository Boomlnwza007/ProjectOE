using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CheckNextB2FSM : BaseState
{
    public CheckNextB2FSM(FSMBoss2EnemySM stateMachine) : base("Swarm", stateMachine) { }
    public List<int> rNumber = new List<int> { 1, 2, 3, 4 };
    // Start is called before the first frame update
    public override void Enter()
    {
        var state = (FSMBoss2EnemySM)stateMachine;
        if (state.phase)
        {
            if (rNumber.Count <= 0)
            {
                rNumber = new List<int> { 1, 2, 3};
            }
            state.merge.stateAtk.Clear();
            int index = Random.Range(0, rNumber.Count);
            int selectedAttack = rNumber[index];
            rNumber.RemoveAt(index);
            state.merge.stateAtk.Add(selectedAttack);
            ChangState(state.merge);
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
