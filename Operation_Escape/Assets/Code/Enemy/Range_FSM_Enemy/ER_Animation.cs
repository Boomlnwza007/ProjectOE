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
        // ปรับมุมให้อยู่ในช่วง 0 ถึง 360 องศา
        targetAngle = (targetAngle + 360) % 360;

        // ตรวจสอบช่วงมุมแล้วกำหนดทิศทางตามที่ต้องการ
        if (targetAngle >= 90 && targetAngle < 112.5f)
        {
            // ขวาบน
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", 1);
        }
        else if (targetAngle >= 112.5f && targetAngle < 157.5f)
        {
            // ขวาบน 45
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", 0.5f);
        }
        else if (targetAngle >= 157.5f && targetAngle < 202.5f)
        {
            // ขวา
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", 0);
        }
        else if (targetAngle >= 202.5f && targetAngle < 247.5f)
        {
            // ขวาล่าง 45
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", -0.5f);
        }
        else if (targetAngle >= 247.5f && targetAngle < 270f)
        {
            // ขวาล่าง
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", -1);
        }
        else if (targetAngle >= 270f && targetAngle < 292.5f)
        {
            // ซ้ายล่าง
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", -1);
        }
        else if (targetAngle >= 292.5f && targetAngle < 337.5f)
        {
            // ซ้ายล่าง 45
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", -0.5f);
        }
        else if (targetAngle >= 337.5f || targetAngle < 22.5f)
        {
            // ซ้าย
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", 0);
        }
        else if (targetAngle >= 22.5f && targetAngle < 67.5f)
        {
            // ซ้ายบน 45
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", 0.5f);
        }
        else if (targetAngle >= 67.5f && targetAngle < 90f)
        {
            // ซ้ายบน
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", 1);
        }

        // ตรวจสอบความเร็วเพื่อกำหนดการเดิน
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

