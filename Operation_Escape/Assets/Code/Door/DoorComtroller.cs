using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComtroller : MonoBehaviour
{
    [SerializeField] private AutoDoor door;
    public int key;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (door.locked)
        {
            if (PlayerControl.control.key.Contains(key))
            {
                door.Unlock();
            }
        }
    }
}
