using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMinion2EnemySM : StateMachine , IDamageable
{
    BaseAnimEnemy animator;
    GameObject bullet;

    [Header("Charge")]
    public LayerMask raycastMaskWay;
    public LayerMask raycastMask;
    public float jumpLength = 6;

    [HideInInspector]
    public M2AttackFSM attack;
    [HideInInspector]
    public M2IdleFSM idle;

    public bool imortal { get; set; }

    private void Awake()
    {
        attack = new M2AttackFSM(this);
        idle = new M2IdleFSM(this);
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
