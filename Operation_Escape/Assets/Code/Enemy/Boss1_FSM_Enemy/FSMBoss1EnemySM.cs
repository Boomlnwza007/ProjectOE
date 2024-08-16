using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FSMBoss1EnemySM : StateMachine, IDamageable
{
    [Header("Range")]
    [SerializeField] private float laserDistance = 100;
    [SerializeField] public GameObject bulletmon;
    public Transform laserGun;
    public Transform laserFireStart;
    public LineRenderer m_lineRenderer;
    public Gradient laserColorGradientOriginal;
    public Gradient laserColorGradient;
    public LayerMask obstacleLayer;
    private bool laserHitZone;
    private bool laserFiring;
    public float speedRot = 10f;

    [Header("Melee")]
    public Transform hand;
    public Transform handStart;
    public GameObject[] meleeHitZone;

    [Header("State")]
    public int Health;
    public int dmg;
    public float Speed;
    public float visRange;
    public bool cooldown;
    public bool overdrive;
    public bool overdriveChang;
    public int overdriveGageMax;
    private int overdriveGage;
    private float overdriveTime;
    public float overdriveTimer = 60;
    private float time;
    public float timeCooldown = 2f;
    public float fireRate = 0.8f;
    public IAiAvoid ai;
    public bool imortal { get; set; }
    [SerializeField] LootTable lootDrop;
    public Transform target;
    public Rigidbody2D rb;
    public string stateName;
    [HideInInspector]
    public CheckDistanceB1FSM checkDistanceState;
    [HideInInspector]
    public DashAB1FSM dashAState;
    [HideInInspector]
    public IdleB1FSM idleState;
    [HideInInspector]
    public NormalAB1FSM normalAState;
    [HideInInspector]
    public RangeAB1Fsm rangeAState;
    [HideInInspector]
    public OverdriveChangFSM overdriveChangState;
    private void Awake()
    {
        ai = gameObject.GetComponent<IAiAvoid>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        ai.target = target;
        ai.Maxspeed = Speed;
        checkDistanceState = new CheckDistanceB1FSM(this);
        idleState = new IdleB1FSM(this);
        normalAState = new NormalAB1FSM(this);
        dashAState = new DashAB1FSM(this);
        rangeAState = new RangeAB1Fsm(this);
        overdriveChangState = new OverdriveChangFSM(this);
    }

    //protected override BaseState GetInitialState()
    //{
    //    return idleState;
    //}

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

        if (overdrive)
        {
            overdriveTime += Time.deltaTime;
            if (overdriveTime > overdriveTimer)
            {
                overdriveGage = 0;
                overdriveTime = 0;
                overdrive = false;
            }
        }

        if (laserHitZone)
        {
            LaserHitZone();
        }

        if (laserFiring)
        {
            LaserFire();
        }
    }

    public async UniTask MeleeHitzone(float charge,int hitZone)
    {
        GameObject HitZone = Instantiate(meleeHitZone[hitZone], handStart.parent);
        Collider2D _colliders = HitZone.GetComponent<Collider2D>();
        await FadeMelee(HitZone.GetComponent<SpriteRenderer>(),charge);

        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.SetLayerMask(LayerMask.GetMask("Player"));
        Physics2D.OverlapCollider(_colliders, filter, colliders);
        foreach (var hit in colliders)
        {
            IDamageable player = hit.GetComponent<IDamageable>();
            if (hit.CompareTag("Player"))
            {
                player.Takedamage(dmg, DamageType.Rang, 0);
            }
        }
        
        await UniTask.WaitForSeconds(0.5f);
        Destroy(HitZone);        
    }

    public async UniTask FadeMelee(SpriteRenderer spriteRenderer, float duration )
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            color.a = newAlpha;
            spriteRenderer.color = color;
            await UniTask.Yield();
        }
        color = Color.white;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    public void MeleeFollow()
    {
        Vector2 dir = (target.position - hand.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = hand.eulerAngles.z;

        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));

        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);
        hand.eulerAngles = new Vector3(0, 0, newAngle);

    }

    public void LaserFollow()
    {
        Vector2 dir = (target.position - laserGun.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = laserGun.eulerAngles.z;

        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);

        laserGun.eulerAngles = new Vector3(0, 0, newAngle);

    }

    public async UniTask ShootLaser(float charge, float duration,float speedMulti)
    {
        float speedRotOri = speedRot;
        speedRot = speedRot * speedMulti;
        m_lineRenderer.colorGradient = laserColorGradient;
        laserHitZone = true;
        m_lineRenderer.enabled = true;

        await FadeLaser(charge, laserColorGradient, false);
        m_lineRenderer.colorGradient = laserColorGradientOriginal;
        laserFiring = true;

        await UniTask.WaitForSeconds(duration);

        await FadeLaser(0.2f, laserColorGradientOriginal, true);
        laserHitZone = false;
        m_lineRenderer.enabled = false;
        laserFiring = false;
        speedRot = speedRotOri;
    }

    [ContextMenu(nameof(ShootMissile))]
    public async UniTask ShootMissile()
    {
        Vector2 directionToPlayer = (ai.target.position - transform.position).normalized;
        float angleDirectionToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        float angleLeft = angleDirectionToPlayer + 90;
        float angleRight = angleDirectionToPlayer - 90;

        float spreadAngle = 10;
        int bulletCount = 3;
        float startAngle = -spreadAngle * ((bulletCount - 1) / 2.0f);
        await UniTask.WhenAll(Miissile(bulletCount, startAngle, spreadAngle, angleLeft), Miissile(bulletCount, startAngle, -spreadAngle, angleRight));
    }

    public async UniTask Miissile(int bulletCount,float startAngle,float spreadAngle,float angleLR)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (spreadAngle * i) + angleLR;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject bulletG = Instantiate(bulletmon, transform.position, rotation);
            bulletG.GetComponent<BulletFollow>().target = ai.target;
            await UniTask.WaitForSeconds(0.1f);
        }
    }

    public void DrawRay(Vector2 startPos,Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    private async UniTask FadeLaser(float charge, Gradient colorGradient,bool fadeOut)
    {
        float elapsedTime = 0f;
        int fadeStart = 0;
        int fadeEnd = 1;
        if (fadeOut)
        {
            fadeStart = 1;
            fadeEnd = 0;
        }
        GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;

        while (elapsedTime < charge)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Lerp(fadeStart, fadeEnd, elapsedTime / charge);

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = alphaValue;
            }

            Gradient gradient = new Gradient();
            gradient.SetKeys(colorGradient.colorKeys, alphaKeys);
            m_lineRenderer.colorGradient = gradient;

            await UniTask.Yield();
        }
    }

    public void LaserHitZone()
    {
        if (Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right, laserDistance, obstacleLayer))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right, laserDistance, obstacleLayer);
            DrawRay(laserFireStart.position, _hit.point);
        }
        else
        {
            DrawRay(laserFireStart.position, laserFireStart.position + laserFireStart.transform.right * laserDistance);
        }
    }

    public void LaserFire()
    {

        RaycastHit2D[] hitInfo = Physics2D.BoxCastAll(laserFireStart.position, new Vector2(m_lineRenderer.startWidth, m_lineRenderer.startWidth), 0f, laserGun.right, laserDistance);

        foreach (var hit in hitInfo)
        {
            if (hit.collider.CompareTag("Player"))
            {
                IDamageable player = hit.collider.GetComponent<IDamageable>();
                if (player != null)
                {
                    ////////Edit
                    player.Takedamage(dmg, DamageType.Rang, 0);                    
                }
            }
        }
        LayerMask playerLayer = LayerMask.GetMask("Player");
        if (Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right);
            Debug.Log(_hit.collider.name);
            DrawRay(laserFireStart.position, _hit.point);
        }
        else
        {
            DrawRay(laserFireStart.position, laserFireStart.position + laserFireStart.transform.right * laserDistance);
        }
        

    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        overdriveGage++;
        if (overdriveGage == overdriveGageMax)
        {
            overdriveChang = true;
        }
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

}
