using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public GameObject door;
    public bool locked = true;
    private bool opening;
    public int key;
    public TriggerDoor plateSC;

    public void Open()
    {
        door.SetActive(false);
        opening = true;
    }

    public void Close()
    {
        if (opening)
        {
            door.SetActive(true);
            opening = false;
        }
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
