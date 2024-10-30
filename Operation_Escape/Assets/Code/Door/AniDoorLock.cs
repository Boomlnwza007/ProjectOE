using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AniDoorLock : TypeAniDoor
{
    public int speedFrame = 20;
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
        Open().Forget();
    }

    public async UniTask Open()
    {
        for (int i = 0; i < spriteDoorOri_L.Length; i++)
        {
            spriteDoor[0].sprite = spriteDoorOri_L[i];
            spriteDoor[1].sprite = spriteDoorOri_R[i];
            await UniTask.DelayFrame(speedFrame);
        }
    }

    public override void Closesing()
    {
        Close().Forget();
    }

    public async UniTask Close()
    {
        for (int i = spriteDoorOri_L.Length-1; i >=0 ; i--)
        {
            spriteDoor[0].sprite = spriteDoorOri_L[i];
            spriteDoor[1].sprite = spriteDoorOri_R[i];
            await UniTask.DelayFrame(speedFrame);
        }
    }

    public override void Lock()
    {
        spriteDoorOri_L = spriteDoorLock_L;
        spriteDoorOri_R = spriteDoorLock_R;
        spriteDoor[0].sprite = spriteDoorOri_L[0];
        spriteDoor[1].sprite = spriteDoorOri_R[0];
    }

    public override void UnLock()
    {
        spriteDoorOri_L = spriteDoorUnlock_L;
        spriteDoorOri_R = spriteDoorUnlock_R;
        spriteDoor[0].sprite = spriteDoorOri_L[0];
        spriteDoor[1].sprite = spriteDoorOri_R[0];
    }
}
