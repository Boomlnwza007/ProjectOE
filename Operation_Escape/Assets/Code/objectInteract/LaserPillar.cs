using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPillar : MonoBehaviour, IBulletInteract
{
    [SerializeField] private LaserDrone laserDrone;

    public void Interact(DamageType type)
    {       
        laserDrone.Destroy();
    }

}
