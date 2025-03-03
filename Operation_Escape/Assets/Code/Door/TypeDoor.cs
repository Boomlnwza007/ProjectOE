using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeDoor : MonoBehaviour
{
    public DoorSound sound;
    private bool opening;
    public void Open() 
    {
        Openning();
        opening = true;
    }
    public void Close() 
    {
        if (opening)
        {
            Closesing();
            opening = false;
        }
    }

    public virtual void Locking(bool locked) { }
    protected virtual void Openning() { }
    protected virtual void Closesing() { }


}
