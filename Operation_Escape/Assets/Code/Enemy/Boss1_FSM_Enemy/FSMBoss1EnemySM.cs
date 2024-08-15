using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FSMBoss1EnemySM : StateMachine, IDamageable
{
    [Header("Laser")]
    [SerializeField] private float laserDistance = 100;
    public Transform laserGun;
    public Transform laserFireStart;
    public LineRenderer m_lineRenderer;
    public Gradient laserColorGradientOriginal;
    public Gradient laserColorGradient;
    public LayerMask obstacleLayer;
    private bool laserHitZone;
    private bool laserFiring;

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
    public bool startOverdrive;
    public bool overdrive;
    public int overdriveGageMax;
    private int overdriveGage;
    private float overdriveTime;
    public float overdriveTimer = 60;
    private float time;
    public float timeCooldown = 2f;
    public float fireRate = 0.8f;
    public IAiAvoid ai;
    public float speedRot = 10f;
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
    }

    protected override BaseState GetInitialState()
    {
        return dashAState;
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

        if (overdrive)
        {
            overdriveTimer += Time.deltaTime;
            if (overdriveTimer > overdriveTime)
            {
                time = 0;
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

    public async UniTask ChangeMode()
    {
        ai.destination = Vector2.zero;
        startOverdrive = false;
        await UniTask.WaitForSeconds(3f);
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

        if (angleDifference > 5f)
        {
            // ค่อยๆ เปลี่ยนมุมการหมุน
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);
            hand.eulerAngles = new Vector3(0, 0, newAngle);
        }
        else
        {
            // หมุนไปยังมุมเป้าหมายทันที
            hand.eulerAngles = new Vector3(0, 0, targetAngle);
        }
    }

    public void LaserFollow()
    {
        Vector2 dir = (target.position - laserGun.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = laserGun.eulerAngles.z;

        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));

        if (angleDifference > 5f)
        {
            // ค่อยๆ เปลี่ยนมุมการหมุน
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);
            laserGun.eulerAngles = new Vector3(0, 0, newAngle);
        }
        else
        {
            // หมุนไปยังมุมเป้าหมายทันที
            laserGun.eulerAngles = new Vector3(0, 0, targetAngle);
        }
    }

    public async UniTask ShootLaser(float charge ,float duration)
    {
        m_lineRenderer.colorGradient = laserColorGradient;
        laserHitZone = true;
        m_lineRenderer.enabled = true;

        await FadeLaser(charge,laserColorGradient,false);
        m_lineRenderer.colorGradient = laserColorGradientOriginal;

        laserFiring = true;

        await UniTask.WaitForSeconds(duration);

        await FadeLaser(0.2f,laserColorGradientOriginal,true);
        laserHitZone = false;
        m_lineRenderer.enabled = false;
        laserFiring = false;

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

        if (Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right);
            DrawRay(laserFireStart.position, _hit.point);
        }

    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        overdriveGage++;
        if (overdriveGage == overdriveGageMax)
        {
            overdrive = true;
            startOverdrive = true;
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
