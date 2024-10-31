using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : BaseGun
{
    public GameObject bulletUltiPrefab;
    public override void Fire()
    {
        ammo--;
        if (canUltimate)
        {
            Instantiate(bulletUltiPrefab, bulletTranform.position, bulletTranform.rotation);
            if (ammo <= 0)
            {
                var playerCombat = PlayerControl.control.playerCombat;
                playerCombat.ReUltimate();
            }
        }
        else
        {
            Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
        }
        firing = false;
    }

    public override void RemoveUltimate()
    {
        firing = false;
    }

    public override void Setup()
    {
        ammo = maxAmmo;
        firing = true;
        fireRate = 0;
        SetUpbulletTranform();
    }

    public override void Ultimate()
    {
        canUltimate = true;
        ammo = 2;
    }
}
