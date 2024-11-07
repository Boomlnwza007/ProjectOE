using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ShieldGun : BaseGun
{
    public GameObject bulletUltiPrefab;
    private bool shoot;
    [Header("Shield")]
    public GameObject shieldPrefab;
    private ShieldGun_Shield shield;
    public int MaxhpShield = 200;
    public static int hpShield;


    public override void Enter()
    {
        Debug.Log(1);
        shield = Instantiate(shieldPrefab, PlayerControl.control.transform).GetComponent<ShieldGun_Shield>();
        shield.Wake(true);
        shield.gameObject.transform.localPosition = Vector3.zero;
    }

    public override void Exit()
    {
        Destroy(shield.gameObject);
        shield.Wake(false);
    }

    public override void Fire()
    {       
        if (canUltimate)    
        {
            Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
            CinemachineControl.Instance.ShakeCamera(1f, 0.2f);

            firing = false;
        }
        else
        {
            if (!shoot)
            {
                Shoot().Forget();
            }

        }
    }

    private async UniTask Shoot()
    {
        shoot = true;
        shield.ShieldOn(false);
        for (int i = 0; i < 3; i++)
        {
            await UniTask.WaitForSeconds(0.1f); 
            ammo--;
            CinemachineControl.Instance.ShakeCamera(1f, 0.2f);

            Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
            PlaySound(sound.shoot);
        }
        shield.ShieldOn(true);
        firing = false;
        shoot = false;


    }

    public override void RemoveUltimate()
    {
        firing = false;
        maxFireRate = 1f;
    }

    public override void Ultimate()
    {
        maxFireRate = 0.1f;
        canUltimate = true;
        ammo = maxAmmo;
    }

    public override void Setup()
    {
        ammo = maxAmmo;
        hpShield = MaxhpShield;
        firing = true;
        fireRate = 0;
        SetUpbulletTranform();
    }
}
