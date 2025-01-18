using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBoss2EnemySM : StateMachine, IDamageable
{
    [Header("status")]
    public AreaEnermy areaEnermy;
    public Boss2Mark areaMark;
    public Collider2D colliderBoss;

    [Header("status")]
    public bool inRoom;

    [HideInInspector] 
    public IdleB2FSM idle;
    [HideInInspector]
    public StrikeB2FSM strike;
    [HideInInspector]
    public BurrowB2FSM burrow;
    [HideInInspector]
    public SwarmB2FSM swarm;
    [HideInInspector]
    public EatB2FSM eat;

     public bool imortal { get; set; }

    private void Awake()
    {
        idle = new IdleB2FSM(this);
        strike = new StrikeB2FSM(this);
        burrow = new BurrowB2FSM(this);
        swarm = new SwarmB2FSM(this);
        eat = new EatB2FSM(this);
        areaMark.state = this;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Health -= damage;
        spriteFlash?.Flash();
        switch (type)
        {
            case DamageType.Rang:
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
        //Instantiate(gunDrop, gameObject.transform.position, Quaternion.identity);
        if (areaEnermy != null)
        {
            areaEnermy.Die(this);
        }
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
    }
}
