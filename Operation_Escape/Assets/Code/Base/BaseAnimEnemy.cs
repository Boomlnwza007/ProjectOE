using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimEnemy : MonoBehaviour
{
    public Animator animator;
    public int layerIndex;
    public Rigidbody2D rb;
    private string currentAnimaton;
    public float timeplay;
    public bool endAnim;

    public float TimePlayer()
    {
        return animator.GetCurrentAnimatorClipInfo(layerIndex).Length;
    }

    public void ChangeAnimationAttack(string newAnimation)
    {
        endAnim = false;
        animator.Play(newAnimation, layerIndex);
        timeplay = animator.GetCurrentAnimatorClipInfo(layerIndex).Length;
        currentAnimaton = newAnimation;
    }

    public void EndAnimation()
    {
        endAnim = true;
    }
}
