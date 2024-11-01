using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceGun : BaseGun
{
    public GameObject bulletUltiPrefab;

    public override void Fire()
    {
        ammo--;
        if (canUltimate)
        {
            //BaseBullet bullet = Instantiate(bulletUltiPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BaseBullet>();
            //PlaySound(sound.shootUltimate);

            //if (ammo <= 0)
            //{
            //    var playerCombat = PlayerControl.control.playerCombat;
            //    playerCombat.ReUltimate();
            //}
        }
        else
        {
            BaseBullet bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BaseBullet>();
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
