using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public int damage;
    public float speed;
    public float knockbackForce;
    public string tagUse;
    public Transform target;
    public bool ready;
    public bool ultimate;

    public virtual void ResetGameObj() {}

    public void KnockBackPush(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackDirection = collision.transform.position - transform.position;
            knockbackDirection.Normalize();

            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
