using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMREnemySM : StateMachine, IDamageable
{
    [SerializeField] 
    private GameObject bullet;
    [SerializeField]
    public Transform bulletTranform;
    public int Health;
    public int dmg;
    public float Speed;
    public float visRange;
    public bool cooldown;
    private float time;
    public float timeCooldown = 2f;
    public float fireRate = 0.8f;
    public IAiAvoid ai;
    public AreaEnermy areaEnermy;
    public bool imortal { get; set; }
    [SerializeField] LootTable lootDrop;
    public Transform target;
    public Rigidbody2D rb;
    [HideInInspector]
    public WanderRFSM wanderState;
    [HideInInspector]
    public CheckDistanceRFSM checkDistanceState;
    [HideInInspector]
    public CloseAttackRFSM closeAttackState;
    [HideInInspector]
    public NormalAttackRFSM normalAttackState;
    public string stateName; 
    private void Awake()
    {
        ai = gameObject.GetComponent<IAiAvoid>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        ai.target = target;
        ai.Maxspeed = Speed;
        wanderState = new WanderRFSM(this);
        checkDistanceState = new CheckDistanceRFSM(this);
        closeAttackState = new CloseAttackRFSM(this);
        normalAttackState = new NormalAttackRFSM(this);
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
    }

    public void Fire()
    {
        GameObject bulletG = Instantiate(bullet, bulletTranform.position, bulletTranform.rotation);
        bulletG.GetComponent<BulletFollow>().target = ai.target;
    }

    public void FireClose()
    {
        float spreadAngle = 10;
        int bulletCount = 3;
        float startAngle = -spreadAngle * ((bulletCount - 1) / 2.0f); ;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (spreadAngle * i);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject bulletG = Instantiate(bullet, bulletTranform.position, bulletTranform.rotation* rotation);
            bulletG.GetComponent<BulletFollow>().target = ai.target;
        }        
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Health -= damage;
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
        ChangState(checkDistanceState);
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
    }
}
