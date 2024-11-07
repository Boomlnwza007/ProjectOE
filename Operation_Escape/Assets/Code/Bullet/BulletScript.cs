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

            Instantiate(expo, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out IBulletInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);

            Instantiate(expo, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Instantiate(expo, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
