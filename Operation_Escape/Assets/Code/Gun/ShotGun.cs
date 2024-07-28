using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : BaseGun
{
    int bulletCount = 5;
    public float spreadAngle = 10f;
    public override void Fire()
    {
        float startAngle = -spreadAngle * ((bulletCount - 1) / 2.0f); ;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (spreadAngle * i);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation * rotation);            
        }
        
    }
}
