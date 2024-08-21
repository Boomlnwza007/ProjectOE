using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGun : BaseGun
{
    public bool charge;
    BulletCharge bullet;

    private void Awake()
    {
        charge = false;
    }

    public override void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!charge&&firing)
            {
                charge = true;
                bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BulletCharge>();
                bullet.charging = true;
                bullet.follow = bulletTranform;
            }
        }
    }

    public override void Special()
    {
        if (charge)
        {
            if (bullet != null)
            {
                bullet.Charge();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                ammo--;
                firing = false;
                charge = false;
                bullet.charging = false;
                bullet.rb.velocity = bullet.transform.right * bullet.speed;
            }
        }

        
    }
}
