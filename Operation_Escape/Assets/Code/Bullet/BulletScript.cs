using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : BaseBullet
{
    public GameObject expo;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ready = true;
        rb.velocity = transform.right * speed;
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

            Expo();
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out IBulletInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);
            Expo();

        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Expo();
            Destroy(gameObject);
        }
    }

    public void Expo()
    {
        if (expo != null)
        {
            Instantiate(expo, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
