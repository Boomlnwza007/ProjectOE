using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1AniControl : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string currentAnimaton;
    public float timeplay;
    public GameObject BossBody;
    public bool endAnim;

    public void ChangeAnimationState(string newAnimation)
    {
        endAnim = false;
        animator.Play(newAnimation, 0);
        currentAnimaton = newAnimation;
    }

    public void EndAnimation()
    {
        endAnim = true;
    }
}
