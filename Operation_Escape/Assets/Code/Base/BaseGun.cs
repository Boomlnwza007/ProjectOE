using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    public string Name;
    public GameObject bulletPrefab;
    public Transform bulletTranform;
    public int damage;
    public float fireRate;
    public float timeReload;
    public int maxAmmo;
    public int bnergyUse;
    public abstract void Fire();
}
