using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart3EnemySM : FSMBaseBoss2EnemySM , IDamageable
{
    [Header("Cooldown")]
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    public BaseAnimEnemy animator;
    public bool imortal { get; set; }

    [HideInInspector]
    public H3IdleFSM Idle;
    [HideInInspector]
    public H3AttackFSM attack;
    [HideInInspector]
    public H3SummonFSM summon;

    private void Awake()
    {
        ResetPositions();
        Idle = new H3IdleFSM(this);
        attack = new H3AttackFSM(this);
        summon = new H3SummonFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return Idle;
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
                lootDrop.InstantiateLoot(dropChange);
                break;
        }
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
    }
}
