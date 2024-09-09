using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : BaseGun
{
    public override void Fire()
    {
        ammo--;
        Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
        firing = false;
    }

    public override void Setup()
    {
        ammo = maxAmmo;
        firing = true;
        fireRate = 0;
    }

    public override void Ultimate()
    {
        throw new System.NotImplementedException();
    }
}
