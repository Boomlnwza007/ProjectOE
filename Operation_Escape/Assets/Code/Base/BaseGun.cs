using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    public string Name;
    public GameObject gunPrefab;
    public GameObject bulletPrefab;
    public Sprite iconGun;
    public Transform bulletTranform;
    [HideInInspector]public float fireRate;
    public float timeReload;
    public float maxFireRate;
    public int ammo;
    public int maxAmmo;
    public int energyUse;
    public bool canSpecial;
    public bool firing = true;
    public bool canUltimate;
    public float timeUltimate;

    private void Awake()
    {
        ammo = maxAmmo;
        firing = true;
        fireRate = 0;
    }

    public abstract void Fire();
    public virtual void Special() { }
    public abstract void Ultimate();
    public abstract void RemoveUltimate();
    public virtual void Remove() { }
    public virtual void Setup() { }


}
