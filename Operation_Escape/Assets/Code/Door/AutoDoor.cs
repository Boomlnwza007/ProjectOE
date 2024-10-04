using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public TypeDoor door;
    public bool locked = true;
    public int key;
    public TriggerDoor plateSC;
    public AreaEnermy area;

    public void Open()
    {
        door.Open();
    }

    public void Close()
    {
        door.Close();
    }

    public void Unlock()
    {
        Destroy(plateSC);
        locked = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (locked)
        {
            if (PlayerControl.control.key.Contains(key))
            {
                locked = false;
                Open();
            }
        }
        else
        {
            Open();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Close();
    }
}
