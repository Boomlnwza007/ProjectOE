using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public TypeDoor door;
    public TypeAniDoor aniDoor;
    public bool locked = true;
    public int key;
    public TriggerDoor plateSC;
    public AreaEnermy area;

    private void Start()
    {
        if (aniDoor != null)
        {
            if (locked)
            {
                aniDoor.Lock();
            }
            else
            {
                aniDoor.UnLock();
            }
        }        
    }

    public void Open()
    {
        if (aniDoor != null)
        {
            door.Open();
        }
        aniDoor.Openning();
    }

    public void Close()
    {
        if (aniDoor != null)
        {
            door.Close();
        }
        aniDoor.Closesing();
    }

    public void Unlock()
    {
        Destroy(plateSC);
        if (aniDoor != null)
        {
            aniDoor.UnLock();
        }
        locked = false;
    }

    public void Lock()
    {
        if (aniDoor != null)
        {
            aniDoor.Lock();
        }
        if (area != null)
        {
            area.Lock();
        }
        else
        {
            locked = true;
        }
    }

    public void LockOn()
    {
        locked = true;
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
