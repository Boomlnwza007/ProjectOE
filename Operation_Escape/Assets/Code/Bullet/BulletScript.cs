using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : BaseBullet
{  

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        ready = true;
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
                target.Takedamage(damage, DamageType.Rang,knockbackForce);
                KnockBackPush(collision);
            }
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out IBulletInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);
            Destroy(gameObject);
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
