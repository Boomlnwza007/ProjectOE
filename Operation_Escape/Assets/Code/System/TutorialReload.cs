using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialReload : MonoBehaviour
{
    public bool On;
    private void Update()
    {
        if (On)
        {
            var currentGun = PlayerControl.control.playerCombat.gunList[PlayerControl.control.playerCombat.currentGun];
            if (currentGun.ammo < currentGun.maxAmmo)
            {
                Tutorial.set.show(7, 3);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!On)
            {
                On = true;
            }
        }
    }
}
