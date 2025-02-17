using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    public bool immortal;

    // Update is called once per frame
    void Update()
    {
        if (immortal)
        {
            if (PlayerControl.control.playerState.health < 50)
            {
                PlayerControl.control.playerState.health = PlayerControl.control.playerState.maxHealth;
            }
        }
    }
}
