using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGun : BaseGun
{
    public GameObject bulletUltiPrefab;
    public override void Fire()
    {
        ammo--;
        if (canUltimate)
        {
            BaseBullet bullet = Instantiate(bulletUltiPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BaseBullet>();
            if (ammo <= 0)
            {
                var playerCombat = PlayerControl.control.playerCombat;
                playerCombat.ReUltimate();
            }
        }
        else
        {
            BaseBullet bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BaseBullet>();

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
    }

    public override void Ultimate()
    {
        canUltimate = true;
        ammo = 4;
    }
}
