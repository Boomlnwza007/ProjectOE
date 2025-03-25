    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_animation : BaseAnimEnemy
{
    public IAiAvoid ai;
    public bool isFacing = true;
    public bool isFacingRight = true;
    private FSMSEnemySM enemySM;

    private void Start()
    {
        ai = gameObject.GetComponent<IAiAvoid>();
        enemySM = gameObject.GetComponent<FSMSEnemySM>();
    }

    private void Update()
    {
        UpdateAnimation();
        animator.SetBool("HasShield", enemySM.shield.conShield && enemySM.shield.redy);
    }

    public void UpdateAnimation()
    {
        Vector2 target = (PlayerControl.control.transform.position - gameObject.transform.position).normalized;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        isFacingRight = angle > -90 && angle < 90;

        if (isFacing)
        {
            animator.SetBool("IsRight", isFacingRight);
            animator.SetFloat("horizon", isFacingRight ? 1 : -1);

        }

        if (rb.velocity != Vector2.zero && !ai.endMove)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }


    }
}
