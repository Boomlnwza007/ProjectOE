using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardShield : MonoBehaviour
{
    public bool canGuard;
    [SerializeField] public Collider2D shield;
    [SerializeField] Transform target;
    [SerializeField] public GameObject bulletPrefab;
    public ContactFilter2D filter;

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
        if (canGuard)
        {            
            gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
            Guard(angle);
        }
        else
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, angle+90);
        }
    }

    public void Guard(float angle)
    {
        List<Collider2D> colliderHit = new List<Collider2D>();
        Physics2D.OverlapCollider(shield, filter, colliderHit);
        foreach (var collider in colliderHit)
        {
            if (collider != shield)
            {
                if (collider.tag == "BulletPlayer")
                {
                    Debug.Log(collider.tag + " " + collider.name);
                    BaseBullet bullet = collider.GetComponent<BaseBullet>();
                    if (bullet != null)
                    {
                        if (bullet.tagUse != "Player")
                        {
                            bullet.target = target;
                            bullet.tagUse = "Player";
                            bullet.rb.velocity = (target.position - transform.position).normalized * bullet.speed;
                            bullet.gameObject.transform.eulerAngles = new Vector3(0, 0, angle * Random.Range(-5f,5f));
                            SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();
                            spriteRenderer.color = Color.red;
                            bullet.ResetGameObj();
                        }                        
                    }
                }
            }
            
        }
        colliderHit.Clear();
    }
}
