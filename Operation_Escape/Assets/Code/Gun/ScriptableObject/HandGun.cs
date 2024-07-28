using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHandgun", menuName = "Gun/Handgun")]
public class HandGun : BaseGun
{
    public override void Fire()
    {
        Instantiate(bulletPrefab, bulletTranform.position, bulletTranform.rotation);
    }    
}
