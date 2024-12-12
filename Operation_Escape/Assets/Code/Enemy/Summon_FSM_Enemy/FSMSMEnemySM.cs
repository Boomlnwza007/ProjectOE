using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSMEnemySM : StateMachine ,IDamageable
{
    public bool imortal { get; set; }
    public AreaEnermy areaEnermy;

    [Header("Cooldown")]
    public bool summonAttack;
    public float cooldownSAttack = 4;
    public bool summonFlee;
    public float cooldownSFlee = 2;

    [Header("Summon")]
    public GameObject drone;
    public int amountDrone;

    [Header("Evasion")]
    public bool stealth;
    public float cooldownStealth = 5;
    public GameObject shield;
    public float cooldownShield = 5;

    [HideInInspector]
    public DistanceSMFSM distance;
    [HideInInspector]
    public SummonSMFSM summon;
    [HideInInspector]
    public WanderSMFSM wander;

    private void Awake()
    {
        wander = new WanderSMFSM(this);
        distance = new DistanceSMFSM(this);
        summon = new SummonSMFSM(this);
        spriteFlash = GetComponent<SpriteFlash>();
    }

    protected override BaseState GetInitialState()
    {
        return wander;
    }

    public void Die()
    {
        if (areaEnermy != null)
        {
            areaEnermy.Die(this);
        }

        ChangState(distance);
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
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

    public override void CombatPhaseOn()
    {
        if (areaEnermy != null)
        {
            areaEnermy.AllcombatPhase();
        }
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
    }
}
