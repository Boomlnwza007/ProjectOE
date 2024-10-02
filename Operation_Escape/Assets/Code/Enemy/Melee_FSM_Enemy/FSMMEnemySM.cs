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
    public EM_Animation animator;

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
        if (areaEnermy == null)
        {
            return;
        }

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

    public void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,2.5f);
        foreach (var hit in colliders)
        {
            IDamageable player = hit.GetComponent<IDamageable>();
            if (hit.CompareTag("Player"))
            {
                player.Takedamage(dmg, DamageType.Melee, 0);
            }
        }
    }
}
