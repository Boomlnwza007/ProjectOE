using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDoor : TypeDoor
{
    public GameObject doorL;
    public GameObject doorR;
    private float sizeDoor;

    private void Start()
    {
        sizeDoor = doorL.GetComponentInChildren<Transform>().localScale.x;
    }

    protected override void Openning()
    {

    }

    protected override void Closesing()
    {

    }

}
