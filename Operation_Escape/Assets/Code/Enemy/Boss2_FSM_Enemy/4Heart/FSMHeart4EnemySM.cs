using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart4EnemySM : FSMBaseBoss2EnemySM ,IDamageable
{
    [Header("Cooldown")]
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    public BaseAnimEnemy animator;
    public bool imortal { get; set; }

    [HideInInspector]
    public H4IdleFSM Idle;
    [HideInInspector]
    public H4AttackFSM attack;
    [HideInInspector]
    public H4SummonFSM summon;

    private void Awake()
    {
        ResetPositions();
        Idle = new H4IdleFSM(this);
        attack = new H4AttackFSM(this);
        summon = new H4SummonFSM(this);
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