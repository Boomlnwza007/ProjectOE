using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFollow : BaseBullet
{
    public float rotationSpeed = 200f;
    public float starTime = 0.5f;
    public float stopTime = 2f;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * force;
        time = 0;
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
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        }
        
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
                target.Takedamage(damage, DamageType.Rang, knockBack);
            }

            Destroy(gameObject);
        }
    }
}