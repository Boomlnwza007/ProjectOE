using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveBullet : BaseBullet
{
    bool hit;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ready = true;
        rb.velocity = transform.right * speed;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse)
        {
            if (!hit)
            {
                hit = true;
                IDamageable target = collision.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.Takedamage(damage, DamageType.Rang, knockbackForce);
                    KnockBackPush(collision);
                }
            }            
        }
    }
}
