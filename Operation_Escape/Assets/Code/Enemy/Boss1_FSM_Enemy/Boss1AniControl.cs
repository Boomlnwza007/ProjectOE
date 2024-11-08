using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1AniControl : MonoBehaviour
{
    public static Boss1AniControl boss1AniControl;
    [SerializeField] private Animator animator;
    private string currentAnimaton;
    public float timeplay;  



    private void Awake()
    {
        boss1AniControl = this;
    }

    public void ChangeAnimationState(string newAnimation)
    {
        // if (currentAnimaton == newAnimation.ToString()) return;

        animator.Play(newAnimation, 0);
        currentAnimaton = newAnimation;
    }

}
