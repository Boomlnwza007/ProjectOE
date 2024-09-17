using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDoor : TypeDoor
{
    public Transform doorL;
    public Transform doorR;
    public float speed;
    public float sizeDoor;
    public bool isLR;
    private Vector3 doorFPosition;
    private Vector3 doorSPosition;
    private Vector3 targetDoorFPosition;
    private Vector3 targetDoorSPosition;
    private bool isOpen;
    private bool isClose;

    private void Awake()
    {
        doorFPosition = doorL.position;
        doorSPosition = doorR.position;

        targetDoorFPosition = doorFPosition;
        targetDoorSPosition = doorSPosition;

        if (isLR)
        {
            targetDoorFPosition.x -= sizeDoor;
            targetDoorSPosition.x += sizeDoor;
        }
        else
        {
            targetDoorFPosition.y += sizeDoor;
            targetDoorSPosition.y -= sizeDoor;
        }
        
    }

    protected override void Openning()
    {
        isOpen = true;
        isClose = false;
    }

    protected override void Closesing()
    {
        isClose = true;
        isOpen = false;
    }

    private void FixedUpdate()
    {
        if (isOpen)
        {
            doorL.position = Vector2.MoveTowards(doorL.position, targetDoorFPosition, speed * Time.deltaTime);
            doorR.position = Vector2.MoveTowards(doorR.position, targetDoorSPosition, speed * Time.deltaTime);
        }

        if (isClose)
        {
            doorL.position = Vector2.MoveTowards(doorL.position, doorFPosition, speed * Time.deltaTime);
            doorR.position = Vector2.MoveTowards(doorR.position, doorSPosition, speed * Time.deltaTime);
        }
    }

}
