using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInRoom : MonoBehaviour
{    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FSMBoss2EnemySM state))
        {
            state.inRoom = true;
        }

        if (collision.gameObject.TryGetComponent(out DummyBoss2 dummy))
        {
            dummy.inRoom = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FSMBoss2EnemySM state))
        {
            state.inRoom = true;
        }

        if (collision.gameObject.TryGetComponent(out DummyBoss2 dummy))
        {
            dummy.inRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FSMBoss2EnemySM state))
        {
            state.inRoom = false;
        }

        if (collision.gameObject.TryGetComponent(out DummyBoss2 dummy))
        {
            dummy.inRoom = false;
        }
    }
}
