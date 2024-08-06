using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMEnemySM : StateMachine, IDamageable
{
    public int Health;
    public int dmg;
    public float Speed;
    public float visRange;
    public bool cooldown;
    public float time;
    public float timeCooldown = 6f;
    public IAiAvoid ai;
    public AreaEnermy areaEnermy;
    [SerializeField] LootTable lootDrop;
    public Transform target;
    public Rigidbody2D rb;
    [HideInInspector]
    public WanderEMFSM wanderState;
    [HideInInspector]
    public NormalAttackEMFSM N1Attack;
    [HideInInspector]
    public CheckDistanceEMFSM CheckDistance;
    [HideInInspector]
    public ChargeEMFSM Charge;
    public string stateName;

    private void Awake()
    {
        ai = gameObject.GetComponent<IAiAvoid>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        ai.target = target;
        ai.Maxspeed = Speed;
        ai.canMove = true;
        wanderState = new WanderEMFSM(this);
        N1Attack = new NormalAttackEMFSM(this);
        CheckDistance = new CheckDistanceEMFSM(this);
        Charge = new ChargeEMFSM(this);
    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
            stateName = curState.name;
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

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Health -= damage;
        if (Health <= 0)
        {
            switch (type)
            {
                case DamageType.Rang:
                    //lootDrop.InstantiateLoot(3);
                    break;
                case DamageType.Melee:
                    lootDrop.InstantiateLoot(1);
                    break;
            }
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
