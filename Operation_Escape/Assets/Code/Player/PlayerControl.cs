using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private PlayerState PlayerState;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        PlayerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (playerMovement.state == PlayerMovement.State.Dodge)
        {
            playerCombat.canFire = playerMovement.canCombat;
        }
    }
}
