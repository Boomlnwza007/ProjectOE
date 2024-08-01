using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : BaseGun
{
    public override void Fire()
    {
        Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
    }
}
