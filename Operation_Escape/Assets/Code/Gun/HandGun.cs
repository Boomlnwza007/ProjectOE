using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGun : BaseGun
{
    public override void Fire()
    {
        Instantiate(bulletPrefab, bulletTranform.position, Quaternion.identity);
    }
}
