using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    static public PlayerControl control;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private PlayerState playerState;
    [SerializeField] private PlayerAim playerAim;
    [SerializeField] private Animator animator;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerState = GetComponent<PlayerState>();
        control = this;
    }

    private void Update()
    {
        bool isFacingRight = playerAim.angle > -90 && playerAim.angle < 90;

        if (isFacingRight)
        {
            animator.SetBool("Right", false);
        }
        else
        {
            animator.SetBool("Right", true);
        }

        if (playerMovement.horizontal != 0 || playerMovement.vertical != 0)
        {
            animator.speed = 1;
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
            animator.speed = 0;
            animator.SetBool("Move", false);
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
