using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAttackType : MonoBehaviour
{
    protected Animator animator;
    protected StateMachine mon;

    public virtual void DiractionAttack() { }

    public void GetData(Animator animator, StateMachine mon)
    {
        this.animator = animator;
        this.mon = mon;
    }
}
