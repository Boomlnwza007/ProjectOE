using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGun : BaseGun
{
    public override void Fire()
    {
        ammo--;
        Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
        firing = false;
    }

    public override void Ultimate()
    {
        throw new System.NotImplementedException();
    }
}
