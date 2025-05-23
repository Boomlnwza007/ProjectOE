using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBladeScriipt : BaseBullet
{
    private Vector3 startPos;
    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    public float Range;
    public Vector3 originScale;
    public Vector3 finalScale = new Vector3(0.2f, 0.2f, 0);
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        rb.velocity = transform.right * speed;
        originScale = transform.localScale;
        ready = true;
    }

    private void Update()
    {
        float distance = Vector2.Distance(startPos, gameObject.transform.position);
        if (distance >= Range)
        {            
            animator.enabled = true;
        }

        float t = Mathf.Clamp01(distance / Range);
        transform.localScale = Vector3.Lerp(originScale, finalScale, t);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IObjInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);
            animator.enabled = true;
        }        
        else if (collision.tag == tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null && !hitTargets.Contains(target))
            {
                target.Takedamage(damage, DamageType.Rang, knockbackForce);
                KnockBackPush(collision);
                hitTargets.Add(target);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if (!collision.CompareTag("InEdgeWall"))
            {
                animator.enabled = true;
            }
        }
    }

    public override void ResetGameObj()
    {
        startPos = transform.position;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
