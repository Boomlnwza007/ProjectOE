using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMEnemySM : StateMachine, IDamageable
{
    [Header("status")]
    public bool cooldown;
    public float time;
    public float timeCooldown = 6f;
    public AreaEnermy areaEnermy;
    public bool imortal { get; set; }
    public string stateName;
    private SpriteFlash spriteFlash;

    [HideInInspector]
    public WanderEMFSM wanderState;
    [HideInInspector]
    public NormalAttackEMFSM N1Attack;
    [HideInInspector]
    public CheckDistanceEMFSM CheckDistance;
    [HideInInspector]
    public ChargeEMFSM Charge;

    private void Awake()
    {
        wanderState = new WanderEMFSM(this);
        N1Attack = new NormalAttackEMFSM(this);
        CheckDistance = new CheckDistanceEMFSM(this);
        Charge = new ChargeEMFSM(this);
        spriteFlash = GetComponent<SpriteFlash>();
    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
            stateName = curState.nameState;
        }
        if (cooldown)
        {
            time += Time.deltaTime;
            if (time > timeCooldown)
            {
                time = 0;
                cooldown = false;
            }
        }
    }

    protected override BaseState GetInitialState()
    {
        return wanderState;
    }

    public void CooldownApproching()
    {
        float _cooldown = 5;
        List<StateMachine> enemy = areaEnermy.enemy;
        foreach (var item in enemy)
        {
            if (item.TryGetComponent<FSMMEnemySM>(out FSMMEnemySM fSMM))
            {
                if (fSMM != this)
                {
                    if (!fSMM.cooldown)
                    {
                        fSMM.cooldown = true;
                        fSMM.time = _cooldown;
                        _cooldown--;
                    }
                }
            }
        }
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
                lootDrop.InstantiateLoot(1);
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
        return null;
    }

    public override void CombatPhaseOn()
    {
        ChangState(CheckDistance);
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
    }
}
