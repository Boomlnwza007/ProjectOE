using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBounce_Dmg : MonoBehaviour
{
    public GameObject bullet;
    public BulletBounce bulletBounce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == bulletBounce.tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                target.Takedamage(bulletBounce.damage, DamageType.Rang, bulletBounce.knockbackForce);
                bulletBounce.KnockBackPush(collision);
            }
            Destroy(bullet);
        }
        else if (collision.TryGetComponent(out IBulletInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);
            Destroy(bullet);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            bulletBounce.bounceCount++;

            if (bulletBounce.bounceCount >= bulletBounce.maxBunceCount)
            {
                if (!bulletBounce.ultimate)
                {
                    Destroy(bullet);
                    return;
                }
            }

            if (!bulletBounce.ultimate)
            {
                bulletBounce.damage *= 2;
            }
            else
            {
                bulletBounce.damage += 10;
            }
        }
    }
}
