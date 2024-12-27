using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMinion1EnemySM : StateMachine , IDamageable
{
    BaseAnimEnemy animator;

    [Header("Charge")]
    public LayerMask raycastMaskWay;
    public LayerMask raycastMask;
    public float jumpLength = 6;

    [HideInInspector]
    public M1AttackFSM attack;
    [HideInInspector]
    public M1IdleFSM idle;
    [HideInInspector]
    public M1checkDistanceFSM checkDistance;

    public bool imortal { get; set; }

    private void Awake()
    {
        attack = new M1AttackFSM(this);
        idle = new M1IdleFSM(this);
        checkDistance = new M1checkDistanceFSM(this);
    }

    public void Run(float multiply)
    {
        ai.maxspeed *= multiply;
        animator.animator.speed *= multiply;
    }

    public void Walk()
    {
        ai.maxspeed = Speed;
        animator.animator.speed = 1;
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
