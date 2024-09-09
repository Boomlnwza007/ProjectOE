using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGun : BaseGun
{
    public bool charge = false;
    BulletCharge bullet;

    private void Awake()
    {
        charge = false;
    }

    public override void Fire()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!charge&&firing)
            {
                charge = true;
                bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BulletCharge>();
                bullet.charging = true;
                bullet.follow = bulletTranform;
                Debug.Log("Fire " + bullet.name);
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
                PlayerControl.control.Slow(50);
                if (Input.GetButtonUp("Fire1"))
                {
                    ammo--;
                    firing = false;
                    charge = false;
                    bullet.charging = false;
                    bullet.rb.velocity = bullet.transform.right * bullet.speed;
                    PlayerControl.control.Slow(0);
                }
            }



        }
        
    }

    public override void Ultimate()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        charge = false;
        ammo = maxAmmo;
        firing = true;
        fireRate = 0;
    }

    public override void Remove()
    {
        if (bullet != null)
        {
            firing = false;
            charge = false;
            bullet.charging = false;
            bullet.rb.velocity = bullet.transform.right * bullet.speed;
            PlayerControl.control.Slow(0);
        }        
    }
}
