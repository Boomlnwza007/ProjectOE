using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloDoor : TypeDoor
{
    public GameObject door;
    protected override void Openning()
    {
        door.SetActive(false);
    }

    protected override void Closesing()
    {
        door.SetActive(true);
    }
}
