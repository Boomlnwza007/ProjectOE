using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EM_Animation : BaseAnimEnemy
{
    public IAiAvoid ai;
    private string currentAnimaton;
    public bool isFacing = true;
    public bool isFacingRight = true;
    public AttackCollider[] hitZone;
    public GameObject hitZoneBar;

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
        if (isFacing)
        {
            Vector2 target = (PlayerControl.control.transform.position - gameObject.transform.position).normalized;
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            bool isFacingRight = angle > -90 && angle < 90;
            animator.SetBool("IsRight", isFacingRight);

            if (isFacingRight)
            {
                animator.SetFloat("horizon", 1);
                this.isFacingRight = true;
            }
            else
            {
                animator.SetFloat("horizon", -1);
                this.isFacingRight = false;
            }
        }     

        if (rb.velocity.magnitude > 0.05f && !ai.endMove)
        {
            animator.speed = Mathf.Clamp(rb.velocity.magnitude / ai.maxspeed, 0.1f, 1f);
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.speed = 1;
            animator.SetBool("Walk", false);
        }
    } 
}
