using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    public AutoDoor door ;
    public bool isSide;
    public bool isRightOrisDown;
    private Vector3 positionEnter; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        positionEnter = collision.transform.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Vector3 collisionPos = collision.transform.position;
        bool shouldReturn = false;
        if (isSide)
        {            
            shouldReturn = (isRightOrisDown && positionEnter.x > collisionPos.x) ||
                           (!isRightOrisDown && positionEnter.x < collisionPos.x);
        }       
        else
        {            
            shouldReturn = (isRightOrisDown && positionEnter.y < collisionPos.y) ||
                           (!isRightOrisDown && positionEnter.y > collisionPos.y);
        }

        if (shouldReturn)
        {
            return;
        }

        if (!door.locked)
        {
            door.SetLock();
            door.Close();
            if (door.area != null)
            {
                door.area.hasPlayer = true;
            }
        } 
    }

}
