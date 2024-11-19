using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPillar : MonoBehaviour, IBulletInteract, IRestartOBJ
{
    [SerializeField] private LaserDrone laserDrone;

    public void Interact(DamageType type)
    {       
        laserDrone.Destroy();
        AudioManager.audioManager.PlaySFX(laserDrone.breakLaser);
    }

    public void Reset()
    {
        laserDrone.enabled = true;
        laserDrone.Reset();
    }
}
