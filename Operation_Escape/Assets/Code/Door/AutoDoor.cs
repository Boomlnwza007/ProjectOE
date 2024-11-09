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
            aniDoor.Openning();
        }
        door.Open();
    }

    public void Close()
    {
        if (aniDoor != null)
        {
            aniDoor.Closesing();
        }
        door.Close();
    }

    public void Unlock()
    {
        plateSC.gameObject.SetActive(false);
        if (aniDoor != null)
        {
            aniDoor.UnLock();
        }
        locked = false;
    }

    public void UnlockDead()
    {
        if (aniDoor != null)
        {
            aniDoor.UnLock();
        }
        locked = false;
    }

    public void Lock()
    {       

        if (area != null)
        {

            area.Lock();
        }
        else
        {
            if (aniDoor != null)
            {
                aniDoor.Lock();
            }
            locked = true;
        }
    }

    public void LockOn()
    {
        if (aniDoor != null)
        {
            aniDoor.Lock();
        }
        locked = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (locked)
        {
            if (PlayerControl.control.key.Contains(key))
            {
                locked = false;
                Unlock();
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
        if (!locked)
        {
            Close();
        }
    }
}
