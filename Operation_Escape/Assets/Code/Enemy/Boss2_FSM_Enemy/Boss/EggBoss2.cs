using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBoss2 : MonoBehaviour , IDamageable
{
    public int Hp = 20;
    public LootTable lootDrop;
    public SpriteFlash spriteFlash;
    public ID minion;
    public float time = 3;
    private float timer = 0;
    public bool imortal { get; set; }

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
                Hp-= damage;
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

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= time)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 3f;
                Instantiate(minion.Item[2], dropPosition ,Quaternion.identity);
                FSMBoss2EnemySM.minionHave++;
            }
            Destroy(gameObject);
        }
    }
}
