using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour, IObjInteract
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
                lootDrop?.InstantiateLoot(1);
                spriteFlash?.Flash();
                Hp--;
                if (Hp <= 0)
                {
                    if (gun != null)
                    {
                        Instantiate(gun,gameObject.transform.position, Quaternion.identity);
                    }
                    Destroy(gameObject);
                }
                break;
            case DamageType.Rang:
                Hp--;
                if (Hp <= 0)
                {
                    Destroy(gameObject);
                }
                spriteFlash.Flash();
                break;
        }
    }



}
