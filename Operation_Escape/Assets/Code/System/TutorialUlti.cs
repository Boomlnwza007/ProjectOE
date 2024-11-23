using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUlti : MonoBehaviour
{
    public bool On;
    private void Update()
    {
        if (On)
        {
            if (PlayerControl.control.playerState.ultimateEnergy == 10)
            {
                Tutorial.set.show(3, 7);
                Destroy(gameObject);
            }
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!On)
            {
                On = true;
            }
        }
    }

}
