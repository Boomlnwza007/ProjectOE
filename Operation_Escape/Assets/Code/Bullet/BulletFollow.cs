using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFollow : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform target;
    public float rotationSpeed = 1f;
    private Quaternion targetRotation;
    public float time;
    public int damage;
    public float force;
    public float knockBack;
    public string tagUse;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * force;
        time = 0;
    }

    private void FixedUpdate()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        targetRotation = rotation;
       
        time += Time.deltaTime;
        if (time > 1f)
        {
            transform.rotation = transform.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        rb.velocity = transform.right * force;

        //// เช็คว่ามุมปัจจุบันใกล้เคียงกับมุมที่ต้องการหรือไม่
        //if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        //{
        //    // เมื่อหมุนถึงมุมที่ต้องการแล้ว หยุดการหมุน
        //    
        //}
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
                target.Takedamage(damage, DamageType.Rang, knockBack);
            }

            Destroy(gameObject);
        }
    }
}
