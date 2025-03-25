using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSEnemySM : StateMachine, IDamageable
{
    [Header("status")]
    public bool cooldownChargeAttack = true;
    public bool cooldownSlamAttack = true;
    public AreaEnermy areaEnermy;
    public bool imortal { get; set; }
    public string stateName;
    public ES_animation animator;

    [Header("status")]
    public bool stun;
    public float timeStunCooldown = 2;
    [HideInInspector] public float timeStun = 0;

    [Header("circle")]
    private float timeCircle;
    public float radius = 10;
    public float offset = 2;
    public LayerMask raycastMaskWalk;

    [Header("Charge")]
    public LayerMask raycastMaskWay;
    public LayerMask raycastMask;
    public float jumpLength = 20;
    public float forcePush = 100;
    public bool dash;
    public float dodgeMaxSpeed = 50f;
    public float dodgeMinimium = 30f;
    public float dodgeSpeedDropMultiplier = 5f;
    public float dodgeStopRange = 5f;
    [HideInInspector] public float rollSpeed;

    [Header("shild")]
    [SerializeField]public GuardShield shield;

    [HideInInspector]
    public CheckDistanceSFSM checkDistanceState;

    [HideInInspector]
    public ChargeAttack chargeAttState;

    [HideInInspector]
    public SlamAttackSFSM slamAttState;

    [HideInInspector]
    public NoShieldSFSM NoShieldState;    

    [HideInInspector]
    public WanderSFSM wanderState;

    private void Awake()
    {
        slamAttState = new SlamAttackSFSM(this);
        NoShieldState = new NoShieldSFSM(this);
        checkDistanceState = new CheckDistanceSFSM(this);
        chargeAttState = new ChargeAttack(this);
        wanderState = new WanderSFSM(this);
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
        if (areaEnermy != null)
        {
            areaEnermy.Die(this);
        }

        Destroy(gameObject);
        Instantiate(deadBody, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>().flipX = animator.isFacingRight;
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

    public void Movement()
    {
        if (Vector2.Distance(transform.position, target.position) < 10)
        {
            Vector2 dir = target.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 15, raycastMaskWalk);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                timeCircle += Time.deltaTime;
                Vector2 normal = (ai.position - target.position).normalized;
                if (timeCircle > 3)
                {
                    offset *= -1;
                    timeCircle = 0;
                    radius = Mathf.Lerp(radius, Random.Range(5, 10), 0.5f);
                }
                Vector2 tangent = new Vector2(-normal.y, normal.x);
                ai.destination = (Vector2)target.position + normal * radius + tangent * offset;
            }
            else
            {
                ai.destination = target.position;
            }
        }
        else
        {
            ai.destination = target.position;
        }      
        
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
}
