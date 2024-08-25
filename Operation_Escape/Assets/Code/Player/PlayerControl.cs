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

    public void EnableInput(bool enable)
    {
        playerMovement.enabled = enable;
        playerCombat.enabled = enable;
    }
}
