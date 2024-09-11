using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGun : MonoBehaviour
{
    public GameObject gunUI;
    public Image image;
    private int curgun = -1;
    PlayerCombat gun;

    private void Start()
    {
        gun = PlayerControl.control.playerCombat;
    }

    private void Update()
    {
        if (gun.gunList.Count>0)
        {
            if (!gunUI.activeSelf)
            {
                gunUI.SetActive(true);
            }

            if (curgun != gun.currentGun)
            {                
                image.sprite = gun.gunList[gun.currentGun].iconGun;
                curgun = gun.currentGun;
            }
        }
        else
        {
            if (gunUI.activeSelf)
            {
                gunUI.SetActive(false);
            }
        }
    }
}
