using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour, IDamageable
{
    public int Hp = 1;
    public LootTable lootDrop;
    public GameObject gun;
    public SpriteFlash spriteFlash;

    public bool imortal { get; set; }

    public void Die()
    {
        Destroy(gameObject);
        if (gun != null)
        {
            Instantiate(gun,transform.position,Quaternion.identity);
        }
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Hp -= damage;
        spriteFlash?.Flash();
        switch (type)
        {
            case DamageType.Rang:
                break;
            case DamageType.Melee:
                lootDrop.InstantiateLoot(0);
                break;
        }
        if (Hp <= 0)
        {
            Die();
        }
    }
}
