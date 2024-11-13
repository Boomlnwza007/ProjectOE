using UnityEngine;

public class BaseState
{
    public string nameState;
    protected StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.nameState = name;
        this.stateMachine = stateMachine;
    }
    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }
    public virtual void ChangState(BaseState Nextstate) 
    {
        if (stateMachine.curState == this)
        {
            stateMachine.ChangState(Nextstate);
        }
    }
}
