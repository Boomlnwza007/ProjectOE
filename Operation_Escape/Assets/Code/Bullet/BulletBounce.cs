using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBounce : BaseBullet
{
    public int maxBunceCount = 3;
    public int bounceCount = 0;
    public bool ultimate;

    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        ready = true;
        rb.velocity = transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IBulletInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);
            Destroy(gameObject);
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            bounceCount++;

            if (bounceCount > maxBunceCount)
            {
                if (!ultimate)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            if (!ultimate)
            {
                damage *= 2;
            }
            else
            {
                damage += 10;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                target.Takedamage(damage, DamageType.Rang, knockbackForce);
                KnockBackPush(collision);
            }
            Destroy(gameObject);
        }

        
    }
}
