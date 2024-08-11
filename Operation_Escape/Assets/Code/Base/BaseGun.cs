using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    public string Name;    
    public GameObject gunPrefab;
    public GameObject bulletPrefab;
    public GameObject iconGun;
    public Transform bulletTranform;
    public float fireRate;
    public float timeReload;
    public float maxFireRate;
    public int ammo;
    public int maxAmmo;
    public int energyUse;
    public bool canSpecial;
    public bool firing = true;

    public abstract void Fire();
    public abstract void Special();


}
