using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState curState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curState = GetInitialState();
        if (curState != null)
        {
            curState.Enter();
        }
    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (curState != null)
        {
            curState.UpdatePhysics();
        }
    }

    public void ChangState(BaseState newState)
    {
        curState.Exit();
        curState = newState;
        curState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    public virtual object Getdata(string key)
    {
        return null;
    }

    private void OnGUI()
    {
        string content = curState.name;
        GUILayout.Label($"<color='red'><size=48>{content}</size></color>");
    }

    public virtual void combatPhaseOn() { }
    public virtual void setCombatPhase(AreaEnermy area) { }
}
