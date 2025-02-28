using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMonH4 : BaseBullet
{
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void lunch()
    {
        Vector2 direction = (Prefire(PlayerControl.control.transform, transform, speed) - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
        ready = true;
    }

    public Vector2 Prefire(Transform target, Transform bulletTransform, float bulletSpeed)
    {
        Vector2 toTarget = (Vector2)target.position - (Vector2)bulletTransform.position;

        float timeToTarget = toTarget.magnitude / bulletSpeed;

        Vector2 predictedPosition = (Vector2)target.position + (Vector2)target.GetComponent<Rigidbody2D>().velocity * timeToTarget;

        return predictedPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse && ready)
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
