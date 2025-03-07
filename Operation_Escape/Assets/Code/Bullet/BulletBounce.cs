using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBounce : BaseBullet
{
    public int maxBunceCount = 3;
    public int bounceCount = 0;
    private float time = 0;
    public float timer = 3;

    void Start()
    {
        ready = true;
        rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        if (ultimate)
        {
            time += Time.deltaTime;
            if (time > timer)
            {
                Destroy(gameObject);
                Expo();
            }
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            bounceCount++;

            if (bounceCount > maxBunceCount)
            {
                if (!ultimate)
                {
                    Destroy(gameObject);
                    Expo();
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
            if (collision.gameObject.TryGetComponent(out IObjInteract bulletInteract))
            {
                bulletInteract.Interact(DamageType.Rang);
            }
            Destroy(gameObject);
            Expo();
        }        
    }

    public override void ResetGameObj()
    {
        bounceCount = 0;
    }
}
