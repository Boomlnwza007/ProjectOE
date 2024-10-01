using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1AniControl : MonoBehaviour
{
    public static Boss1AniControl boss1AniControl;
    [SerializeField] private Animator animator;
    private bool isAttacking;
    private bool isAttackPressed;
    private string currentAnimaton;
    //public string[] stateName = {
    //    "StartDash",
    //    "StopDash",
    //    "AfterDash",
    //    "RangeAtk",
    //    "PreAtk1-2",
    //    "StartAtk1-2",
    //    "PreAtk3",
    //    "Atk3",
    //    "Atk4",
    //    "Wait"
    //};

    public enum StateBoss
    {
        StartDash,
        StopDash,
        AfterDash,
        RangeAtk,
        PreAtk1_2,         
        Atk1_2,
        PreAtk3,
        Atk3,
        Atk4,
        Wait
    }

    private void Awake()
    {
        boss1AniControl = this;
    }

    public void ChangeAnimationState(StateBoss newAnimation)
    {
        if (currentAnimaton == newAnimation.ToString()) return;

        animator.Play(newAnimation.ToString());
        currentAnimaton = newAnimation.ToString();
    }

}
