using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSEnemySM : StateMachine, IDamageable
{
    [Header("status")]
    public bool cooldown;
    public float time;
    public float timeCooldown = 6f;
    public AreaEnermy areaEnermy;
    public bool canGuard;
    public bool imortal { get; set; }
    public string stateName;
    public ES_animation animator;

    [Header("shild")]
    [SerializeField]public GuardShield shield;

    [HideInInspector]
    public BashSFSM bashState;
    [HideInInspector]
    public ChargSFSM chargState;
    [HideInInspector]
    public CheckDistanceSFSM checkDistanceState;
    [HideInInspector]
    public DefendSFSM defendState;
    [HideInInspector]
    public WanderSFSM wanderState;
    private SpriteFlash spriteFlash;

    private void Awake()
    {
        bashState = new BashSFSM(this);
        chargState = new ChargSFSM(this);
        checkDistanceState = new CheckDistanceSFSM(this);
        defendState = new DefendSFSM(this);
        wanderState = new WanderSFSM(this);
        spriteFlash = GetComponent<SpriteFlash>();
    }

    protected override BaseState GetInitialState()
    {
        return wanderState;
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
        Guard();
    }

    public void Guard()
    {
        if (canGuard)
        {
            shield.canGuard = canGuard;
        }
        else
        {
            shield.canGuard = canGuard;
        }
    }

    public void CooldownCharg()
    {
        if (areaEnermy == null)
        {
            return;
        }

        float _cooldown = 5;
        List<StateMachine> enemy = areaEnermy.enemy;
        foreach (var item in enemy)
        {
            if (item.TryGetComponent<FSMSEnemySM>(out FSMSEnemySM fSMM))
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
        if (areaEnermy != null)
        {
            areaEnermy.Die();
        }

        ChangState(wanderState);
        gameObject.SetActive(false);

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.5f);
        foreach (var hit in colliders)
        {
            IDamageable player = hit.GetComponent<IDamageable>();
            if (hit.CompareTag("Player"))
            {
                player.Takedamage(dmg, DamageType.Melee, 0);
            }
        }
    }

    public override void Reset()
    {
        attacking = false;
        ChangState(wanderState);
        rb.velocity = Vector3.zero;
        gameObject.transform.position = firstSpawn;
        spriteFlash.ReFlash();
    }
}
