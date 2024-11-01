using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FSMBoss1EnemySM : StateMachine, IDamageable
{
    [Header("Range")]
    [SerializeField] public GameObject bulletmon;
    private List<GameObject> lasers = new List<GameObject>();
    public float speedRot = 10f;
    public GameObject lineRendererPrefab;
    public Transform handGun;

    [Header("Melee")]
    public Transform hand;
    public Transform handStart;
    public GameObject[] meleeHitZone;

    [Header("status")]
    public bool cooldown;
    public bool overdrive;
    public bool overdriveChang;
    public int overdriveGageMax;
    public Transform originPoint;
    [HideInInspector] public int overdriveGage;
    [HideInInspector] public float overdriveTime;
    public float overdriveTimer = 60;
    private float time;
    public float fireRate = 0.8f;
    public bool imortal { get; set; }
    public string stateName;
    private SpriteFlash spriteFlash;
    public UIBoss uiBoss;

    [Header("Animation")]
    [SerializeField] public Animator animator;

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
        checkDistanceState = new CheckDistanceB1FSM(this);
        idleState = new IdleB1FSM(this);
        normalAState = new NormalAB1FSM(this);
        dashAState = new DashAB1FSM(this);
        rangeAState = new RangeAB1Fsm(this);
        overdriveChangState = new OverdriveChangFSM(this);
        spriteFlash = GetComponentInChildren<SpriteFlash>();
    }

    protected override BaseState GetInitialState()
    {
        return idleState;
    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
            stateName = curState.nameState;
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

        DiractionAttack();
    }

    public void CreatLaserGun()
    {
        SetupHandGun();
        GameObject laser = Instantiate(lineRendererPrefab, transform.position, Quaternion.identity, handGun);
        LaserFire laserg = laser.GetComponent<LaserFire>();
        laserg.SetStartFollow(target.position);
        lasers.Add(laser);
    }

    public void CreatLaserGunFollow()
    {
        SetupHandGun();
        Quaternion topRotation = handGun.rotation * Quaternion.Euler(0, 0, 90);
        GameObject laser = Instantiate(lineRendererPrefab, transform.position, topRotation, handGun);
        LaserFire laserg = laser.GetComponent<LaserFire>();
        laserg.targetPlayer = handGun;
        //laserg.SetovershootAngle(5, target);
        lasers.Add(laser);

        Quaternion bottomRotation = handGun.rotation * Quaternion.Euler(0, 0, -90);
        GameObject laser2 = Instantiate(lineRendererPrefab, transform.position, bottomRotation, handGun);
        LaserFire laserg2 = laser2.GetComponent<LaserFire>();
        laserg2.targetPlayer = handGun;
        //laserg2.SetovershootAngle(5, target);
        lasers.Add(laser2);

    }

    public void SetupHandGun()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        handGun.localRotation = Quaternion.Euler(0, 0, targetAngle);
    }

    public void DelLaserGun()
    {
        foreach (var gun in lasers)
        {
            Destroy(gun);
        }
        lasers.Clear();
    }

    public async UniTask ShootLaser(float charge, float duration, float speedMulti, float Atime)
    {
        foreach (var laserGun in lasers)
        {
            LaserFire laser = laserGun.GetComponent<LaserFire>();
            laser.speedRot = speedRot;
            laser.targetPlayer = target;
            laser.followCode = 2;
            await UniTask.WhenAll(laser.ShootLaser(charge, duration, speedMulti), laser.Aim(Atime));            
        }        
    }

    public async UniTask ShootLaser(float charge, float duration, float speedMulti, float Atime ,float speedRot)
    {
        foreach (var laserGun in lasers)
        {
            LaserFire laser = laserGun.GetComponent<LaserFire>();
            laser.speedRot = speedRot;
            laser.targetPlayer = target;
            laser.followCode = 2;
            await UniTask.WhenAll(laser.ShootLaser(charge, duration, speedMulti), laser.Aim(Atime));
        }
    }

    public async UniTask ShootLaserFollowIn(float charge, float duration, float speedMulti, float Atime)
    {
        CreatLaserGunFollow();

        for (int i = 0; i < lasers.Count; i++)
        {
            LaserFire laser = lasers[i].GetComponent<LaserFire>();
            laser.speedRot = speedRot;
            laser.target = target.position;
            laser.followCode = 0;
            UniTask.WhenAll(laser.ShootLaser(charge, duration, speedMulti), laser.Aim(Atime)).Forget();
        }
        await UniTask.WaitForSeconds(charge);

        for (int i = 0; i < lasers.Count; i++)
        {
            LaserFire laser = lasers[i].GetComponent<LaserFire>();
            laser.accelerationTime = (duration-charge)/2;
        }

        await UniTask.WaitForSeconds(duration + charge);
        handGun.localRotation = Quaternion.Euler(0,0,0);
    }

    public async UniTask MeleeHitzone(float charge, float duration, int hitZone)
    {
        animator.SetTrigger("PreAttack"); //ง้าง
        GameObject HitZone = Instantiate(meleeHitZone[hitZone], handStart.parent);
        Collider2D _colliders = HitZone.GetComponent<Collider2D>();
        await FadeMelee(HitZone.GetComponent<SpriteRenderer>(),charge);      
        animator.SetTrigger("PreAttack"); //จบง้าง

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
        
        await UniTask.WaitForSeconds(duration);
        animator.SetTrigger("Attack");//จบตี
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
        targetAngle += 45;
        targetAngle = (targetAngle + 360) % 360;
        int segment = Mathf.FloorToInt(targetAngle / 90);

        float newAngle = Mathf.LerpAngle(currentAngle, segment*90, Time.deltaTime * speedRot*2);
        hand.eulerAngles = new Vector3(0, 0, newAngle);


        //Debug.Log(angle);
       // hand.eulerAngles = new Vector3(0, 0, segment * 90);

    }

    public async UniTask RangeFollow(float time)
    {
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            Vector2 dir =  (target.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float currentAngle = handGun.eulerAngles.z;
            for (int i = 0; i < lasers.Count; i++)
            {
                LaserFire laser = lasers[i].GetComponent<LaserFire>();
                laser.target = target.position;
            }
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);
            handGun.eulerAngles = new Vector3(0, 0, newAngle);

            if (timer > time)
            {
                break;
            }

            await UniTask.Yield();
        }
    }

    [ContextMenu(nameof(ShootMissile))]
    public async UniTask ShootMissile()
    {
        Vector2 directionToPlayer = (ai.targetTransform.position - transform.position).normalized;
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
            bulletG.GetComponent<BulletFollow>().target = ai.targetTransform;
            await UniTask.WaitForSeconds(0.1f);
        }
    }   

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        overdriveGage += damage;
        if (overdriveGage >= overdriveGageMax && !overdrive)
        {
            overdriveChang = true;
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
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public void JumpCenter()
    {
        transform.position = originPoint.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (handGun.right).normalized * 5);
    }

    public void DiractionAttack()
    {
        Vector2 dir = (target.position - gameObject.transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bool isFacingRight = targetAngle > -90 && targetAngle < 90;
        animator.SetBool("isRight", isFacingRight);
        targetAngle += 45;
        targetAngle = (targetAngle + 360) % 360;
        int segment = Mathf.FloorToInt(targetAngle / 90);

        switch (segment)
        {
            case 0: // ด้านขวา
                animator.SetFloat("horizon", 1);
                animator.SetFloat("vertical", 0);
                break;

            case 1: // ด้านบน
                animator.SetFloat("horizon", isFacingRight ? 1 : -1);
                animator.SetFloat("vertical", 1);
                animator.SetBool("isUp", true);
                break;

            case 2: // ด้านซ้าย
                animator.SetFloat("horizon", -1);
                animator.SetFloat("vertical", 0);
                break;

            case 3: // ด้านล่าง
                animator.SetFloat("horizon", isFacingRight ? 1 : -1);
                animator.SetFloat("vertical", -1);
                animator.SetBool("isUp", false);
                break;

            default:
                Debug.LogError("segment value: " + segment);
                break;
        }

        if (rb.velocity != Vector2.zero && !ai.endMove)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }
}
