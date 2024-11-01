using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceGun : BaseGun
{
    public GameObject bulletUltiPrefab;

    public override void Fire()
    {
        ammo--;
        CinemachineControl.Instance.ShakeCamera(1f, 0.2f);

        if (canUltimate)
        {
            BulletBounce bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BulletBounce>();
            bullet.ultimate = true;
            PlaySound(sound.shoot);
        }
        else
        {
            BulletBounce bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BulletBounce>();
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
