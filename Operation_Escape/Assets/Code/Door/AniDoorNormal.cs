using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniDoorNormal : TypeAniDoor
{
    [SerializeField] private SpriteRenderer[] spriteDoor;
    [SerializeField] private Sprite[] spriteDoorLock;
    [SerializeField] private Sprite[] spriteDoorUnlock;

    public override void Openning()
    {
    }

    public override void Closesing()
    {
    }

    public override void Lock()
    {
        for (int i = 0; i < spriteDoor.Length; i++)
        {
            spriteDoor[i].sprite = spriteDoorLock[i];
        }
    }

    public override void UnLock()
    {
        for (int i = 0; i < spriteDoor.Length; i++)
        {
            spriteDoor[i].sprite = spriteDoorUnlock[i];
        }
    }
}
