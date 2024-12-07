using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardShield : MonoBehaviour
{
    public bool canGuard;
    private bool conShield = true;
    [SerializeField] public SpriteRenderer shieldSprite;
    [SerializeField] public Collider2D shieldCollider;
    [SerializeField] Transform target;
    [SerializeField] public GameObject bulletPrefab;
    public ContactFilter2D filter;
    public float cooldown;
    private float timeCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (conShield)
        {
            shieldCollider.enabled = canGuard;
            shieldSprite.enabled = canGuard;

            if (canGuard)
            {
                //gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
                Guard(angle);
            }
            else
            {
                timeCooldown += Time.deltaTime;
                if (timeCooldown > cooldown)
                {
                    canGuard = true;
                    timeCooldown = 0;
                }
            }
        }        
    }

    public void Guard(float angle)
    {
        List<Collider2D> colliderHit = new List<Collider2D>();
        Physics2D.OverlapCollider(shieldCollider, filter, colliderHit);
        foreach (var collider in colliderHit)
        {
            if (collider == shieldCollider) continue;

            if (!collider.CompareTag("BulletPlayer")) continue;

            BaseBullet bullet = collider.GetComponent<BaseBullet>();
            if (bullet == null || bullet.tagUse == "Player" || !bullet.ready) continue;

            HandleBulletInteraction(bullet, angle);            
        }

        colliderHit.Clear();
    }

    public void HandleBulletInteraction(BaseBullet bullet, float angle)
    {
        if (bullet is BulletCharge)
        {
            BreakShield();
            Destroy(bullet.gameObject);
        }
        else if (bullet.ultimate)
        {
            Destroy(bullet.gameObject);
        }
        else
        {
            bullet.target = target;
            bullet.tagUse = "Player";

            bullet.gameObject.transform.eulerAngles = new Vector3(0, 0, angle + Random.Range(-10f, 10f));
            bullet.rb.velocity = bullet.gameObject.transform.right * bullet.speed;
            SpriteRenderer spriteRenderer = bullet.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
            bullet.ResetGameObj();
        }
    }

    public void ShieldIsOn(bool on)
    {
        conShield = on;
        shieldCollider.gameObject.SetActive(on);
    }

    public void BreakShield()
    {
        canGuard = false;
    }
}
