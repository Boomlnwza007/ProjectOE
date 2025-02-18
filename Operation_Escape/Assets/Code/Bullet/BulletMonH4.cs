using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMonH4 : BaseBullet
{
    private SpriteRenderer sprite;
    private bool shoot;
    private float yTarget;
    private float yPos;
    private bool yPosSet = false;
    public float distance = 10;
    public float offset = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        yPos = transform.position.y;
    }

    public void lunch()
    {
        rb.velocity = transform.up * speed;
        ready = true;
    }

    private void Update()
    {
        if (ready)
        {
            if (Mathf.Abs(transform.position.y - yPos) > distance && !yPosSet)
            {
                Debug.Log(transform.position.y + " " + yPos);
                yPos = transform.position.y;
                yPosSet = true;
            }
            else if (yPosSet)
            {
                if (!sprite.isVisible && !shoot)
                {
                    if (transform.position.y > yPos + offset)
                    {
                        yTarget = CinemachineControl.Instance.player.position.y;
                        transform.position = new Vector3(CinemachineControl.Instance.player.position.x, transform.position.y, transform.position.z);
                        rb.velocity = -transform.up * speed;
                        shoot = true;

                        // Debug.Log(transform.position.y + " "+yPos + 10);
                    }
                    return;
                }
                else if (transform.position.y <= yTarget && shoot)
                {
                    Destroy(gameObject);
                }
            }
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse && shoot)
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
