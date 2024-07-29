using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : ScriptableObject
{
    public string Name;    
    public GameObject gunPrefab;
    public GameObject bulletPrefab;
    public GameObject iconGun;
    public Transform bulletTranform;
    public int damage;
    public float fireRate;
    public float timeReload;
    public int ammo;
    public int maxAmmo;
    public int energyUse;

    public abstract void Fire();  


}
