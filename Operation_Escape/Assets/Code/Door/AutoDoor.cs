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
    public bool UnLock;

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
        AudioManager.audioManager.PlaySFX(door.sound.doorOpen);
    }

    public void Close()
    {
        if (aniDoor != null)
        {
            aniDoor.Closesing();
        }
        door.Close();
        AudioManager.audioManager.PlaySFX(door.sound.doorClose);
    }

    public void Unlock()
    {
        plateSC.gameObject.SetActive(false);
        if (aniDoor != null)
        {
            aniDoor.UnLock();
        }
        locked = false;
        door.Locking(false);
    }

    public void UnlockDead()
    {
        plateSC.gameObject.SetActive(true);
        if (aniDoor != null)
        {
            aniDoor.UnLock();
        }
        locked = false;
        door.Locking(false);
    }

    public void SetLock()
    {      
        if (area != null)
        {
            area.Lock();
            door.Locking(true);
        }
        else
        {
            LockOn();
            door.Locking(true);
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

    public void ForceLock()
    {
        if (!UnLock)
        {
            LockOn();
        }
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
