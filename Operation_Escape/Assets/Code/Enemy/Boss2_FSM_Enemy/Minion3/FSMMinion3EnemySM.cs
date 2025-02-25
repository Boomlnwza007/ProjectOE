using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMinion3EnemySM : StateMachine , IDamageable
{
    BaseAnimEnemy animator;

    [HideInInspector]
    public M3AttackFSM attack;
    [HideInInspector]
    public M3IdleFSM idle;
    [HideInInspector]
    public M3checkDistanceFSM checkDistance;

    public bool imortal { get; set; }

    private void Awake()
    {
        attack = new M3AttackFSM(this);
        idle = new M3IdleFSM(this);
        checkDistance = new M3checkDistanceFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return idle;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Health -= damage;
        spriteFlash.Flash();
        switch (type)
        {
            case DamageType.Rang:
                //lootDrop.InstantiateLoot(3);
                break;
            case DamageType.Melee:
                lootDrop?.InstantiateLoot(dropChange);
                break;
        }
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        FSMBoss2EnemySM.minionHave--;
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(ai.position, 2f);
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out IDamageable dam) && hit.CompareTag("Player"))
            {
                dam.Takedamage(dmg, DamageType.Melee, 5);
            }
        }
    }
}
