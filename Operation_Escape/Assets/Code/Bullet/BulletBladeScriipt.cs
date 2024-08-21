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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        rb.velocity = transform.right * speed;
        originScale = transform.localScale;
    }

    private void Update()
    {
        float distance = Vector2.Distance(startPos, gameObject.transform.position);
        if (distance >= Range)
        {
            Destroy(gameObject);
        }

        float t = Mathf.Clamp01(distance / Range);
        transform.localScale = Vector3.Lerp(originScale, finalScale, t);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null && !hitTargets.Contains(target))
            {
                target.Takedamage(damage, DamageType.Rang,knockBack);
                hitTargets.Add(target);
            }
        }
    }

    public override void ResetGameObj()
    {
        startPos = transform.position;
    }
}
