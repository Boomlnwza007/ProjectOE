using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart1EnemySM : FSMBaseBoss2EnemySM ,IDamageable
{
    [Header("Cooldown")]
    public float timePreSpike;
    public float timeCooldownSpike;

    public BaseAnimEnemy animator;

    [HideInInspector]
    public H1IdleFSM Idle;
    [HideInInspector]
    public H1AttackFSM attack;

    public bool imortal { get; set; }

    private void Awake()
    {
        ResetPositions();
        Idle = new H1IdleFSM(this);
        attack = new H1AttackFSM(this);
        spriteFlash = GetComponent<SpriteFlash>();

    }

    protected override BaseState GetInitialState()
    {
        return Idle;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        if (shield.conShield)
        {
            if (shield.canGuard)
            {
                return;
            }
        }

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
