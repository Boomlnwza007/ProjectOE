using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateBar : MonoBehaviour
{
    [SerializeField] private SliderBar slider;
    [SerializeField] BaseGun gun;
    private PlayerControl Player;
    public string nameGun;
    private void Awake()
    {
        Player = PlayerControl.control;
        slider.SetMax(gun.maxFireRate);
        nameGun = gun.name;
    }

    private void Update()
    {
        if (nameGun != Player.playerCombat.gunList[Player.playerCombat.currentGun].name)
        {
            if (!Player.playerCombat.gunList[Player.playerCombat.currentGun].firing)
            {
                slider.value = Mathf.Lerp(0, Player.playerCombat.gunList[Player.playerCombat.currentGun].maxFireRate, Player.playerCombat.gunList[Player.playerCombat.currentGun].fireRate / Player.playerCombat.gunList[Player.playerCombat.currentGun].maxFireRate);
            }
        }        
    }

}
