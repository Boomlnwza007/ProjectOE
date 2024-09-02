using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFollow : BaseBullet
{
    public float speedChangSpeed = 2f;
    public float startSpeed = 10;
    public float rotationSpeed = 2f;
    public float starTime = 0.5f;
    public float stopTime = 2f;
    private float time;
    private float curSpeed;
    private bool off;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        curSpeed = startSpeed;
        rb.velocity = transform.right * startSpeed;
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

        curSpeed = Mathf.Lerp(curSpeed, speed, Time.deltaTime * speedChangSpeed);

        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
       
        time += Time.deltaTime;
        if (time >= starTime && time <= stopTime && !off)
        {
            if (Vector2.Distance(transform.position,target.position)>2)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                off = true;
            }
        }
        
        rb.velocity = transform.right * curSpeed;

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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
