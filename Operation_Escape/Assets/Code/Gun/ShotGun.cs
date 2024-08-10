using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : BaseGun
{
    public override void Fire()
    {
        ammo--;
        Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
    }

    public override void Special()
    {
        return;
    }
}
