using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBoss2EnemySM : FSMBaseBoss2EnemySM, IDamageable
{
    [Header("status")]
    public Boss2Mark areaMark;
    public Collider2D colliderBoss;

    [Header("status")]
    public bool inRoom;

    [HideInInspector]
    public AreaEnermy areaEnermy;
    [HideInInspector] 
    public IdleB2FSM idle;
    [HideInInspector]
    public StrikeB2FSM strike;
    [HideInInspector]
    public AreaAttackB2FSM area;
    [HideInInspector]
    public SwarmB2FSM swarm;
    [HideInInspector]
    public EatB2FSM eat;
    [HideInInspector]
    public CheckNextB2FSM checkNext;
    [HideInInspector]
    public LaserB2FSM laser;

     public bool imortal { get; set; }

    private void Awake()
    {
        idle = new IdleB2FSM(this);
        strike = new StrikeB2FSM(this);
        area = new AreaAttackB2FSM(this);
        swarm = new SwarmB2FSM(this);
        eat = new EatB2FSM(this);
        checkNext = new CheckNextB2FSM(this);
        laser = new LaserB2FSM(this);
        areaMark.state = this;
        grid = areaMark.grid;
        spawnPoint = areaMark.monSpawn;
    }

    protected override BaseState GetInitialState()
    {
        return idle;
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
