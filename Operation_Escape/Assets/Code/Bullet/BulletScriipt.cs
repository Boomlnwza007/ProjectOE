using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScriipt : BaseBullet
{  

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * force;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                target.Takedamage(damage, DamageType.Rang,knockBack);
            }
            Destroy(gameObject);
        }        
    }
}