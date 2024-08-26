using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    static public PlayerControl control;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private PlayerState PlayerState;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        PlayerState = GetComponent<PlayerState>();
        control = this;
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
