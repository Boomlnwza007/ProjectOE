using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour, IBulletInteract
{
    public int Hp = 1;
    public LootTable lootDrop;
    public GameObject gun;
    public SpriteFlash spriteFlash;

    public void Interact(DamageType type)
    {
        switch (type)
        {           
            case DamageType.Melee:
                lootDrop.InstantiateLoot(1);
                spriteFlash.Flash();
                if (Hp <= 0)
                {
                    if (gun != null)
                    {
                        Instantiate(gun,gameObject.transform.position, Quaternion.identity);
                    }
                    Destroy(gameObject);
                }
                break;
        }
    }



}