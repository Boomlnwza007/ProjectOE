using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMinion : BaseBullet
{
    private SpriteRenderer sprite;
    private bool shoot;
    private float yTarget;
    private float yPos;
    private bool yPosSet = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        rb.velocity = transform.up * speed;
    }

    private void Update()
    {
        if (!sprite.isVisible && !shoot)
        {
            if (!yPosSet)
            {
                yPos = transform.position.y;
                yPosSet = true;
            }

            if (transform.position.y  > yPos + 5)
            {
                ready = true;
                yTarget = CinemachineControl.Instance.player.position.y;
                transform.position = new Vector3(CinemachineControl.Instance.player.position.x, transform.position.y, transform.position.z);
                rb.velocity = -transform.up * speed;
                shoot = true;
            }            
            return;
        }
        else if (transform.position.y <= yTarget && shoot)
        {
            Destroy(gameObject);
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
