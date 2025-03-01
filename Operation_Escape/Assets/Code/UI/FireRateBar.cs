using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateBar : MonoBehaviour
{
    [SerializeField] private SliderBar slider;
    [SerializeField] BaseGun gun;
    private PlayerControl Player;
    public string nameGun;

    [Header("Layer")]
    public Canvas canvas;
    public SpriteRenderer gunSprite;

    private void Start()
    {
        Player = PlayerControl.control;
        nameGun = gun.name;

        if (GunDown())
        {
            slider.slider.value = 0;
            return;
        }

        if (CheckCurGun())
        {
            slider.SetMax(gun.maxFireRate, Player.playerCombat.gunList[Player.playerCombat.currentGun].fireRate);
        }
        else
        {
            slider.SetMax(gun.maxFireRate);
        }
    }

    private void Update()
    {
        if (GunDown() && slider.value != 0)
        {
            slider.slider.value = 0;
        }
        else if (CheckCurGun())
        {
            var currentGun = Player.playerCombat.gunList[Player.playerCombat.currentGun];
            slider.value = Mathf.Lerp(0, currentGun.maxFireRate, currentGun.fireRate / currentGun.maxFireRate);
        }

        if (gunSprite != null)
        {
            if (Player.isFacingRight)
            {
                gunSprite.sortingOrder = 6;
                canvas.sortingOrder = gunSprite.sortingOrder + 1;

            }
            else
            {
                gunSprite.sortingOrder = 3;
                canvas.sortingOrder = gunSprite.sortingOrder + 1;

            }
        }
    }

    private bool CheckCurGun()
    {
        if (Player.playerCombat.gunList.Count == 0)
        {
            return false;
        }

        var currentGun = Player.playerCombat.gunList[Player.playerCombat.currentGun];
        return nameGun == currentGun.name && !currentGun.firing;
    }

    private bool GunDown()
    {
        if (Player.playerCombat.gunList.Count == 0)
        {
            return false;
        }

        var currentGun = Player.playerCombat.gunList[Player.playerCombat.currentGun];
        return currentGun.energyUse > Player.playerState.energy && currentGun.ammo == 0;
    }

}
