using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AniDoorLock : TypeAniDoor
{
    [SerializeField] private SpriteRenderer[] spriteDoor;
    private Sprite[] spriteDoorOri_L;
    private Sprite[] spriteDoorOri_R;

    [SerializeField] private Sprite[] spriteDoorLock_L;
    [SerializeField] private Sprite[] spriteDoorLock_R;

    [SerializeField] private Sprite[] spriteDoorUnlock_L;
    [SerializeField] private Sprite[] spriteDoorUnlock_R;

    private void Start()
    {
        spriteDoorOri_L = spriteDoorUnlock_L;
        spriteDoorOri_R = spriteDoorUnlock_R;
    }

    public override void Openning()
    {
        UniTask.WhenAll(Open(spriteDoor[0], spriteDoorOri_L), Open(spriteDoor[1], spriteDoorOri_R)).Forget();
    }

    public async UniTask Open(SpriteRenderer spriteDoor, Sprite[] spriteDoorNe)
    {
        for (int i = 0; i < spriteDoorNe.Length; i++)
        {
            spriteDoor.sprite = spriteDoorNe[i];
            await UniTask.DelayFrame(10);
        }
    }

    public override void Closesing()
    {
        UniTask.WhenAll(close(spriteDoor[0], spriteDoorOri_L), close(spriteDoor[1], spriteDoorOri_R)).Forget();
    }

    public async UniTask close(SpriteRenderer spriteDoor, Sprite[] spriteDoorNe)
    {
        for (int i = spriteDoorNe.Length-1; i >= 0; i--)
        {
            spriteDoor.sprite = spriteDoorNe[i];
            await UniTask.DelayFrame(10);
        }
    }

    public override void Lock()
    {
        spriteDoorOri_L = spriteDoorLock_L;
        spriteDoorOri_R = spriteDoorLock_R;
    }

    public override void UnLock()
    {
        spriteDoorOri_L = spriteDoorUnlock_L;
        spriteDoorOri_R = spriteDoorUnlock_R;
    }
}
