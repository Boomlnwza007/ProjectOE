using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EM_Animation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public IAiAvoid ai;
    private string currentAnimaton;
    public bool isFacing = true;
    public float timeplay;

    private void Start()
    {
        ai = gameObject.GetComponent<IAiAvoid>();
    }

    private void Update()
    {
        if (isFacing)
        {
            UpdateAnimation();
        }
    }

    public void UpdateAnimation()
    {
        Vector2 target = (PlayerControl.control.transform.position - gameObject.transform.position).normalized;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        bool isFacingRight = angle > -90 && angle < 90;
        animator.SetBool("IsRight", isFacingRight);

        if (isFacingRight)
        {
            animator.SetFloat("horizon", 1);
        }
        else
        {
            animator.SetFloat("horizon", -1);
        }

        if (rb.velocity != Vector2.zero && !ai.endMove)
        {
            animator.speed = 1;
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.speed = 1;
            animator.SetBool("Walk", false);
        }
    }

    public float TimePlayer()
    {
        return animator.GetCurrentAnimatorClipInfo(1).Length;
    }

    public void ChangeAnimationAttack(string newAnimation)
    {
        animator.Play(newAnimation,1);
        timeplay = animator.GetCurrentAnimatorClipInfo(1).Length;
        currentAnimaton = newAnimation;
    }
}
