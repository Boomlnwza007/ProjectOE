using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGun : BaseGun
{
    public override void Fire()
    {
        ammo--;
        BaseBullet bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BaseBullet>();
        if (canUltimate)
        {
            bullet.damage = 50;
            bullet.speed = 25;
            bullet.transform.localScale *= 4;
            if (ammo <= 0)
            {
                var playerCombat = PlayerControl.control.playerCombat;
                playerCombat.ReUltimate();
            }
        }

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
