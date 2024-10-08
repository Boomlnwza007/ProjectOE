using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFollowBoss1 : FollowAttackType
{
    public override void DiractionAttack()
    {
        Vector2 dir = (mon.target.position - gameObject.transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bool isFacingRight = targetAngle > -90 && targetAngle < 90;
        animator.SetBool("isRight", isFacingRight);
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
                animator.SetBool("isUp", true);
                break;

            case 2: // ด้านซ้าย
                animator.SetFloat("horizon", -1);
                animator.SetFloat("vertical", 0);
                break;

            case 3: // ด้านล่าง
                animator.SetFloat("horizon", isFacingRight ? 1 : -1);
                animator.SetFloat("vertical", -1);
                animator.SetBool("isUp", false);
                break;

            default:
                Debug.LogError("segment value: " + segment);
                break;
        }

        if (mon.rb.velocity != Vector2.zero && !mon.ai.endMove)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }
}
