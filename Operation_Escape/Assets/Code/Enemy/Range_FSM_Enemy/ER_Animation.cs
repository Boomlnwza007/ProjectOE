using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ER_Animation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public IAiAvoid ai;
    private string currentAnimaton;
    public bool isFacing = true;
    public bool isFacingRight = true;
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
        Vector2 dir = (PlayerControl.control.transform.position - gameObject.transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bool isFacingRight = targetAngle > -90 && targetAngle < 90;
        animator.SetBool("IsRight", isFacingRight);
        targetAngle += 45;
        targetAngle = (targetAngle + 360) % 360;
        int segment = Mathf.FloorToInt(targetAngle / 90);

        switch (segment)
        {
            case 0: // ด้านขวา
                animator.SetFloat("horizon", 1);
                animator.SetFloat("vertical", 0);
                break;

            case 1: // ด้านบน
                animator.SetFloat("horizon", isFacingRight ? 1 : -1);
                animator.SetFloat("vertical", 1);
                //animator.SetBool("isUp", true);
                break;

            case 2: // ด้านซ้าย
                animator.SetFloat("horizon", -1);
                animator.SetFloat("vertical", 0);
                break;

            case 3: // ด้านล่าง
                animator.SetFloat("horizon", isFacingRight ? 1 : -1);
                animator.SetFloat("vertical", -1);
               // animator.SetBool("isUp", false);
                break;

            default:
                Debug.LogError("segment value: " + segment);
                break;
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
        animator.Play(newAnimation, 1);
        timeplay = animator.GetCurrentAnimatorClipInfo(1).Length;
        currentAnimaton = newAnimation;
    }
}
