using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBoss2 : MonoBehaviour , IDamageable
{
    public int Hp = 20;
    public LootTable lootDrop;
    public SpriteFlash spriteFlash;
    public ID minion;
    public  int minionID = 3;
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
            switch (minionID)
            {
                case 1:
                    SpawnM1();
                    break;
                case 2:
                    SpawnM2();
                    break;
                case 3:
                    SpawnM3();
                    break;
                default:
                    SpawnM3();
                    break;
            }
            Destroy(gameObject);
        }
    }

    public void SpawnM1()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 1f;
            StateMachine m1 = Instantiate(minion.Item[0], dropPosition, Quaternion.identity).GetComponent<StateMachine>();
            FSMBoss2EnemySM.minionHave.Add(m1);
        }
    }

    public void SpawnM2()
    {
        Vector2 dropPosition1 = (Vector2)transform.position + Random.insideUnitCircle * 2f;
        StateMachine m1 = Instantiate(minion.Item[1], dropPosition1, Quaternion.identity).GetComponentInChildren<StateMachine>();
        FSMBoss2EnemySM.minionHave.Add(m1);

        Vector2 dropPosition2;
        do
        {
            dropPosition2 = (Vector2)transform.position + Random.insideUnitCircle * 4f;
        } while (Vector2.Distance(dropPosition1, dropPosition2) < 2f);

        StateMachine m2 = Instantiate(minion.Item[1], dropPosition2, Quaternion.identity).GetComponentInChildren<StateMachine>();
        Debug.Log(m2.name);
        FSMBoss2EnemySM.minionHave.Add(m2);
    }

    public void SpawnM3()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 1f;
            StateMachine m3 = Instantiate(minion.Item[2], dropPosition, Quaternion.identity).GetComponent<StateMachine>();
            FSMBoss2EnemySM.minionHave.Add(m3);
        }
    }

}
