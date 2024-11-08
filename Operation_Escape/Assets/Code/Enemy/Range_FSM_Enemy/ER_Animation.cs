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
        // ��Ѻ����������㹪�ǧ 0 �֧ 360 ͧ��
        targetAngle = (targetAngle + 360) % 360;

        // ��Ǩ�ͺ��ǧ������ǡ�˹���ȷҧ�������ͧ���
        if (targetAngle >= 90 && targetAngle < 112.5f)
        {
            // ��Һ�
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", 1);
        }
        else if (targetAngle >= 112.5f && targetAngle < 157.5f)
        {
            // ��Һ� 45
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", 0.5f);
        }
        else if (targetAngle >= 157.5f && targetAngle < 202.5f)
        {
            // ���
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", 0);
        }
        else if (targetAngle >= 202.5f && targetAngle < 247.5f)
        {
            // �����ҧ 45
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", -0.5f);
        }
        else if (targetAngle >= 247.5f && targetAngle < 270f)
        {
            // �����ҧ
            animator.SetFloat("horizon", 1);
            animator.SetFloat("vertical", -1);
        }
        else if (targetAngle >= 270f && targetAngle < 292.5f)
        {
            // ������ҧ
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", -1);
        }
        else if (targetAngle >= 292.5f && targetAngle < 337.5f)
        {
            // ������ҧ 45
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", -0.5f);
        }
        else if (targetAngle >= 337.5f || targetAngle < 22.5f)
        {
            // ����
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", 0);
        }
        else if (targetAngle >= 22.5f && targetAngle < 67.5f)
        {
            // ���º� 45
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", 0.5f);
        }
        else if (targetAngle >= 67.5f && targetAngle < 90f)
        {
            // ���º�
            animator.SetFloat("horizon", -1);
            animator.SetFloat("vertical", 1);
        }

        // ��Ǩ�ͺ�����������͡�˹�����Թ
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

