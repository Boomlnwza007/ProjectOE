using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyGun : BaseGun
{
    public GameObject bulletUltiPrefab;

    public override void Fire()
    {
        ammo--;
        if (canUltimate)
        {
            ammo++;
            Instantiate(bulletUltiPrefab, bulletTranform.position, bulletTranform.rotation);          
            PlaySound(sound.shootUltimate);
        }
        else
        {
            Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
            PlaySound(sound.shoot);
        }


        firing = false;
    }

    public override void RemoveUltimate()
    {
        firing = false;
    }

    public override void Ultimate()
    {
        canUltimate = true;
        ammo = maxAmmo;
    }

    public override void Setup()
    {
        ammo = maxAmmo;
        firing = true;
        fireRate = 0;
        SetUpbulletTranform();
    }
}
