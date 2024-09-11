using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateBar : MonoBehaviour
{
    [SerializeField] private SliderBar slider;
    [SerializeField] BaseGun gun;
    private PlayerControl Player;
    public string nameGun;

    private void Start()
    {
        Player = PlayerControl.control;
        nameGun = gun.name;
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

            if (CheckCurGun())
            {
                slider.value = Mathf.Lerp(0, Player.playerCombat.gunList[Player.playerCombat.currentGun].maxFireRate, Player.playerCombat.gunList[Player.playerCombat.currentGun].fireRate / Player.playerCombat.gunList[Player.playerCombat.currentGun].maxFireRate);
            }
        
            
    }

    private bool CheckCurGun()
    {
        if (Player.playerCombat.gunList.Count == 0)
        {
            return false;            
        }

        return nameGun == Player.playerCombat.gunList[Player.playerCombat.currentGun].name
           && !Player.playerCombat.gunList[Player.playerCombat.currentGun].firing;
    }

}
