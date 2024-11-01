using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class    Gun : BaseGun
{
    public GameObject bulletUltiPrefab;
    public override void Fire()
    {
        ammo--;
        if (canUltimate)
        {

            firing = false;
        }
        else
        {
            StartCoroutine(Shoot());
        }


    }

    private IEnumerator Shoot()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
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
