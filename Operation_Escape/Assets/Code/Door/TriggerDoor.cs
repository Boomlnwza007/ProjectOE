using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    public AutoDoor door ;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!door.locked)
        {
            door.SetLock();
            door.Close();
            if (door.area != null)
            {
                door.area.hasPlayer = true;
            }
        }        
    }

}
