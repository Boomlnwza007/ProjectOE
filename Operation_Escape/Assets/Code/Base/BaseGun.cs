using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    public string Name;
    public GameObject gunPrefab;
    public GameObject gunULPrefab;
    public GameObject bulletPrefab;
    public Sprite iconGun;
    public Transform bulletTranform;
    public Transform aimTranform;
    public float aimDistance;
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
    public GunSound sound;

    private void Awake()
    {
        ammo = maxAmmo;
        firing = true;
        fireRate = 0;
    }

    public void SetUpbulletTranform()
    {
        aimDistance = Vector2.Distance(gameObject.transform.position, aimTranform.position);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioManager.audioManager.PlaySFX(clip);
        }
    }

    public abstract void Fire();
    public virtual void Special() { }
    public abstract void Ultimate();
    public abstract void RemoveUltimate();
    public virtual void Remove() { }
    public virtual void Setup() { }
    public virtual void Enter() { }
    public virtual void Exit() { }

}
