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
    private bool random;
    public float distance = 10;
    public float offset = 10;
    public GameObject particle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        yPos = transform.position.y;
        rb.velocity = transform.up * speed;
        target = PlayerControl.control.transform;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.y - yPos) > distance && !yPosSet)
        {
            yPos = transform.position.y;
            yPosSet = true;
        }
        else if(yPosSet)
        {
            if (!sprite.isVisible && !shoot)
            {
                if (transform.position.y > yPos + offset)
                {
                    ready = true;
                    yTarget = target.position.y - 3;
                    transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
                    rb.velocity = -transform.up * speed;
                    shoot = true;
                }
                return;
            }
            else if (transform.position.y <= yTarget && shoot)
            {
                Instantiate(particle , transform.position,Quaternion.identity);
                Destroy(gameObject);
            }
            else if (!sprite.isVisible && shoot && !random)
            {
                if (Random.value > 0.5)
                {
                    Vector2 prePosPlayer;
                    float travelTime = Mathf.Abs(transform.position.y - target.position.y) / speed;
                    prePosPlayer = PredictPlayerPosition(travelTime);
                    yTarget = prePosPlayer.y - 3;
                    transform.position = new Vector3(prePosPlayer.x, transform.position.y, transform.position.z);
                }
                else
                {
                    random = true;
                }
            }

        }        
    }

    Vector3 PredictPlayerPosition(float predictionTime)
    {
        Rigidbody2D playerRb = target.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            return target.position + (Vector3)playerRb.velocity * predictionTime;
        }
        return target.position;
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
