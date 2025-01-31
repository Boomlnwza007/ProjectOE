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

    protected virtual void Openning() { }
    protected virtual void Closesing() { }


}
