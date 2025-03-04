using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeObj : MonoBehaviour, IDamageable
{
    public int Hp = 10;
    public LootTable lootDrop;
    public SpriteFlash spriteFlash;
    public LayerMask raycastMask;
    public LayerMask raycastMaskEnemy;

    public bool imortal { get ; set ; }

    public void Die()
    {
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        switch (type)
        {
            case DamageType.Melee:
                lootDrop?.InstantiateLoot(1);
                spriteFlash?.Flash();
                Hp -= damage;
                if (Hp <= 0)
                {
                    Die();
                }
                break;
            case DamageType.Rang:
                Hp -= damage;
                spriteFlash.Flash();
                if (Hp <= 0)
                {
                    Die();
                }
                break;
        }
    }

    public void Check()
    {

        if (Physics2D.OverlapCircle(transform.position, 0.5f, raycastMask))
        {
            Destroy(gameObject);
        }

        GetComponent<SpriteRenderer>().sortingLayerName = "Player";

    }
}
