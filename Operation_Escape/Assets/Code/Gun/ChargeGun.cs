using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChargeGun : BaseGun
{
    public bool charge = false;
    BulletCharge bullet;
    LaserCharge laser;
    public GameObject laserUltiPrefab;
    public GameObject ChangeEffPrefab;
    private GameObject ChangeEff;



    private void Awake()
    {
        charge = false;
    }

    public override void Fire()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!charge&&firing)
            {
                if (canUltimate)
                {
                    charge = true;
                    laser = Instantiate(laserUltiPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<LaserCharge>();
                    laser.bulletTranform = bulletTranform;
                }
                else
                {
                    charge = true;
                    bullet = Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation).GetComponent<BulletCharge>();
                    bullet.charging = true;
                    bullet.follow = bulletTranform;
                    ChangeEff = Instantiate(ChangeEffPrefab, bulletTranform.position, bulletTranform.rotation, bulletTranform);
                }

                PlayLoop().Forget();
            }
        }
    }

    public override void Special()
    {
        if (charge)
        {
            CinemachineControl.Instance.ShakeCamera(1f, 0.2f);
            if (canUltimate)
            {
                PlayerControl.control.Slow(80);
                if (Input.GetButtonUp("Fire1"))
                {
                    ShootUlt();
                    //bullet.charging = false;
                    //PlayerControl.control.Slow(0);
                }
            }
            else if (bullet != null)
            {
                bullet.Charge();
                PlayerControl.control.Slow(50);
                if (Input.GetButtonUp("Fire1"))
                {
                    Shoot();
                }
            }
        }        
    }

    public void Shoot()
    {
        ammo--;
        bullet.ready = true;
        firing = false;
        charge = false;
        bullet.charging = false;
        bullet.rb.velocity = bullet.transform.right * bullet.speed;
        PlayerControl.control.Slow(0);
        PlaySound(sound.shoot);
        Destroy(ChangeEff);
    }

    public void ShootUlt()
    {
        var playerCombat = PlayerControl.control.playerCombat;
        ammo--;
        firing = false;
        charge = false;
        canUltimate = false;
        RemoveUltimate();
        playerCombat.ReUltimate();
        PlaySound(sound.shootUltimate);
    }

    private async UniTask PlayLoop()
    {
        while (charge)
        {
            PlaySound(sound.special[0]);
            await UniTask.Delay((int)(sound.special[0].length * 1000));
        }
    }

    public override void Ultimate()
    {
        Remove();
        canUltimate = true;
        ammo = 1;
    }

    public override void Setup()
    {
        charge = false;
        ammo = maxAmmo;
        firing = true;
        fireRate = 0;
        SetUpbulletTranform();
    }

    public override void Remove()
    {
        if (bullet != null)
        {
            Shoot();
            PlayerControl.control.Slow(0);
            Destroy(ChangeEff);
        }        
    }   

    public override void RemoveUltimate()
    {
        firing = false;
        charge = false;
        Destroy(laser?.gameObject);
        PlayerControl.control.Slow(0);
    }
}
