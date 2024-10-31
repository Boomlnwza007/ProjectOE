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
            }
        }
    }

    public override void Special()
    {
        if (charge)
        {
            if (canUltimate)
            {
                //PlayerControl.control.Slow(50);
                if (Input.GetButtonUp("Fire1"))
                {
                    var playerCombat = PlayerControl.control.playerCombat;
                    ammo--;
                    firing = false;
                    charge = false;
                    canUltimate = false;
                    RemoveUltimate();
                    playerCombat.ReUltimate();

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
                    ammo--;
                    firing = false;
                    charge = false;
                    bullet.charging = false;
                    bullet.rb.velocity = bullet.transform.right * bullet.speed;
                    PlayerControl.control.Slow(0);
                    Destroy(ChangeEff);
                }
            }
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
            firing = false;
            charge = false;
            bullet.charging = false;
            bullet.rb.velocity = bullet.transform.right * bullet.speed;
            PlayerControl.control.Slow(0);
        }        
    }   

    public override void RemoveUltimate()
    {
        firing = false;
        charge = false;
        Destroy(laser.gameObject);
        PlayerControl.control.Slow(0);
    }
}
