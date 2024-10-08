using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFollowNorMon : FollowAttackType
{
    public override void DiractionAttack()
    {
        Vector2 dir = (mon.target.position - gameObject.transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bool isFacingRight = targetAngle > -90 && targetAngle < 90;
        animator.SetBool("IsRight", isFacingRight);       

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
