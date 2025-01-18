using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInRoom : MonoBehaviour
{    
    private FSMBoss2EnemySM state;
    private void Start()
    {
        state = GetComponentInParent<Boss2Mark>().state;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        state.inRoom = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        state.inRoom = false;
    }
}
