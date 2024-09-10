using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Player")]
    static public PlayerControl control;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private PlayerState playerState;
    [SerializeField] private PlayerAim playerAim;
    [SerializeField] private Animator animator;

    [Header("UI")]
    [SerializeField] private SliderBar healthBar;
    [SerializeField] private SliderBar ultimateEnergyBar;
    [SerializeField] private SliderBar EnergyBar;
    [SerializeField] private OBJBar bulletBar;



    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerState = GetComponent<PlayerState>();
        control = this;
    }
    private void Start()
    {
        healthBar.SetMax(playerState.maxHealth, playerState.health);
        EnergyBar.SetMax(playerState.maxEnergt, playerState.energy);
        ultimateEnergyBar.SetMax(playerState.maxUltimateEnergy, playerState.ultimateEnergy);
    }

    private void Update()
    {
        bool isFacingRight = playerAim.angle > -90 && playerAim.angle < 90;

        switch (playerMovement.state)
        {
            case PlayerMovement.State.Normal:

                if (isFacingRight)
                {
                    animator.SetBool("Right", true);
                }
                else
                {
                    animator.SetBool("Right", false);
                }

                if (playerMovement.horizontal != 0 || playerMovement.vertical != 0)
                {
                    if (isFacingRight && playerMovement.horizontal < 0 || !isFacingRight && playerMovement.horizontal > 0)
                    {
                        animator.SetBool("MoveBackwards", true);
                    }
                    else
                    {
                        animator.SetBool("MoveBackwards", false);
                    }
                    animator.SetBool("Move", true);
                }
                else
                {
                    animator.SetBool("Move", false);
                }

                break;
        }

        Bar();
        Gun();
    }

    public void Bar()
    {
        if (playerState.health != healthBar.value)
        {
            healthBar.SetValue(playerState.health);
        }

        if (playerState.energy != EnergyBar.value)
        {
            EnergyBar.SetValue(playerState.energy);
        }

        if (playerState.ultimateEnergy != ultimateEnergyBar.value)
        {
            ultimateEnergyBar.SetValue(playerState.ultimateEnergy);
        }
    }

    public void Gun()
    {
        if (playerCombat.gunList.Count != 0)
        {
            if (playerCombat.gunList[playerCombat.currentGun].ammo != bulletBar.value)
            {
                bulletBar.SetValue(playerCombat.gunList[playerCombat.currentGun].ammo);
            }

            if (bulletBar.obj.nameItem[bulletBar.curgun] != playerCombat.gunList[playerCombat.currentGun].name)
            {
                bulletBar.SetUp(playerCombat.gunList[playerCombat.currentGun].name);
            }
        }

        
    }

    public void EnableInput(bool enable)
    {
        if (!enable)
        {
            playerMovement.rb.velocity = Vector2.zero;
            playerMovement.state = PlayerMovement.State.Normal;
        }
        playerMovement.enabled = enable;
        playerCombat.enabled = enable;
    }

    public void Slow(float percent)
    {
        percent /= 100f;
        playerMovement.slowSpeed = playerMovement.speed * percent;
    }

    public void ResetSlow()
    {
        playerMovement.slowSpeed = 0;
    }
}
