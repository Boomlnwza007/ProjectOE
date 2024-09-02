using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMREnemySM : StateMachine, IDamageable
{
    [Header("Range")]
    [SerializeField] 
    private GameObject bullet;
    [SerializeField]
    public Transform bulletTranform;

    [Header("status")]
    public bool cooldown;
    private float time;
    public float timeCooldown = 2f;
    public float fireRate = 0.8f;
    public AreaEnermy areaEnermy;
    public bool imortal { get; set; }

    [Header("circle")]
    private float timeCircle;
    public float radius = 13;
    public float offset = 2;

    [HideInInspector]
    public WanderRFSM wanderState;
    [HideInInspector]
    public CheckDistanceRFSM checkDistanceState;
    [HideInInspector]
    public CloseAttackRFSM closeAttackState;
    [HideInInspector]
    public NormalAttackRFSM normalAttackState;
    private SpriteFlash spriteFlash;
    public string stateName; 

    private void Awake()
    {
        wanderState = new WanderRFSM(this);
        checkDistanceState = new CheckDistanceRFSM(this);
        closeAttackState = new CloseAttackRFSM(this);
        normalAttackState = new NormalAttackRFSM(this);
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
    }

    public void Fire()
    {
        Vector2 predictedPosition = Prefire(target, bulletTranform, 20);
        GameObject bulletG = Instantiate(bullet, bulletTranform.position, bulletTranform.rotation);
        bulletG.GetComponent<BulletFollow>().target = ai.targetTarnsform;
        if (Random.value > 0.5)
        {
            Vector2 direction = (predictedPosition - (Vector2)bulletTranform.position).normalized;
            bulletG.transform.right = direction;
            bulletG.GetComponent<Rigidbody2D>().velocity = direction * 20;
        }
    }

    public Vector2 Prefire(Transform target, Transform bulletTransform, float bulletSpeed)
    {
        Vector2 toTarget = (Vector2)target.position - (Vector2)bulletTransform.position;

        float timeToTarget = toTarget.magnitude / bulletSpeed;

        Vector2 predictedPosition = (Vector2)target.position + (Vector2)target.GetComponent<Rigidbody2D>().velocity * timeToTarget;

        return predictedPosition;
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
            bulletG.GetComponent<BulletFollow>().target = ai.targetTarnsform;
        }        
    }

    public void Movement()
    {
        Debug.Log(Vector2.Distance(transform.position, target.position));

        if (Vector2.Distance(transform.position,target.position) < 15)
        {
            timeCircle += Time.deltaTime;
            var normal = (ai.position - target.position).normalized;
            if (timeCircle > 3)
            {
                offset *= -1;
                timeCircle = 0;
                radius = Random.Range(5,13);
            }
            var tangent = Vector3.Cross(normal, new Vector3(0, 0, 1));
            ai.destination = target.position + normal * radius + tangent * offset;
        }
        else
        {
            ai.destination = target.position;
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
