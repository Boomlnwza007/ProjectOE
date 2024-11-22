using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCheck : MonoBehaviour
{
    private void Update()
    {
        if (PlayerControl.control.playerState.ultimateEnergy == 10)
        {
            Tutorial.set.show(3, 3);
            Destroy(gameObject);
        }
    }
}
