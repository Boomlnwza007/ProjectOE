using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_animation : BaseAnimEnemy
{
    public IAiAvoid ai;

    private void Start()
    {
        ai = gameObject.GetComponent<IAiAvoid>();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    public void UpdateAnimation()
    {
        Vector2 target = (PlayerControl.control.transform.position - gameObject.transform.position).normalized;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        bool isFacingRight = angle > -90 && angle < 90;

        animator.SetBool("IsRight", isFacingRight);

        if (rb.velocity != Vector2.zero && !ai.endMove)
        {
            animator.speed = 1;
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.speed = 0;
            animator.SetBool("Walk", false);
        }
    }
}
