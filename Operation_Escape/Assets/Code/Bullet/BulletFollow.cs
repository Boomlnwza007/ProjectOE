using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFollow : BaseBullet
{
    public float rotationSpeed = 2f;
    public float starTime = 0.5f;
    public float stopTime = 2f;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        time = 0;
        ready = true;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
       
        time += Time.deltaTime;
        if (time >= starTime && time <= stopTime)
        {
            if (Vector2.Distance(transform.position,target.position)>2)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }           
        }
        
        rb.velocity = transform.right * speed;

    }

    //private void OnBecameInvisible()
    //{
    //    Destroy(gameObject);
    //}

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
