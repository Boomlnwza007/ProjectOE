using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelBulletAll : MonoBehaviour
{
    public LayerMask bulletleLayer;
    public LayerMask BodyLayer;

    public void DestroyBullet()
    {
        Collider2D[] enemygameObject = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, bulletleLayer);
        foreach (var item in enemygameObject)
        {
            Destroy(item.gameObject);
        }
    }

    public void DestroyBody()
    {
        Collider2D[] enemygameObject = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, bulletleLayer);
        foreach (var item in enemygameObject)
        {
            if (item.GetComponent<DeadEnemy>())
            {
                Destroy(item.gameObject);
            }            
        }
    }
}
