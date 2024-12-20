using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCharge : BaseBullet
{
    public float time;
    public float chargeTime = 3;
    public int damageChargeMax = 50;
    public int damageChargeMin = 10;
    public bool charging;
    public Vector3 originScale;
    public Vector3 finalScale = new Vector3(0.7f, 0.7f, 0);
    public float startScale;
    public Transform follow;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        time = 0;
        originScale = transform.localScale;
        startScale = originScale.x;
    }

    public void Charge()
    {
        if (time < chargeTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / chargeTime);
            transform.localScale = Vector3.Lerp(originScale, finalScale, t);
            float _damage = (time) * (damageChargeMax - damageChargeMin) / (chargeTime) + damageChargeMin;
            damage = Mathf.Clamp((int)_damage, damageChargeMin, damageChargeMax);
        }
    }

    private void Update()
    {
        if (charging)
        {
            transform.position = follow.position;
            transform.rotation = follow.rotation;
        }
    }

    public void Shoot()
    {
        rb.velocity = transform.right * speed;
        ready = true;
    }

    private void OnBecameInvisible()
    {
        if (!charging)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!charging)
        {
            if (collision.TryGetComponent(out GuardShield shield) && ready)
            {
                shield.BreakShield();
                return;
            }

            if (collision.tag == tagUse)
            {
                IDamageable target = collision.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.Takedamage(damage, DamageType.Rang, knockbackForce);
                    KnockBackPush(collision);
                    Destroy(gameObject);
                }
            }
            else if (collision.TryGetComponent(out IBulletInteract bulletInteract))
            {
                bulletInteract.Interact(DamageType.Rang);
                Destroy(gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                Destroy(gameObject);
            }
        }
        
    }
}
