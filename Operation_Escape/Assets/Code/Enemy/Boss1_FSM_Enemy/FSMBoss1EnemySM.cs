using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

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
    public float forcePush = 100;
    public GameObject bladeslash;
    public Transform bladeslashTransform;

    [Header("Dash")]
    public bool dash;
    public float dodgeMaxSpeed = 100f;
    public float dodgeMinimium = 50f;
    public float dodgeSpeedDropMultiplier = 5f;
    public float dodgeStopRange = 5f;
    [HideInInspector]public float rollSpeed;

    [Header("status")]
    public bool cooldown;
    public bool overdrive;
    public bool overdriveChang;
    public int overdriveGageMax;
    [HideInInspector] public int overdriveGage;
    [HideInInspector] public float overdriveTime;
    public float overdriveTimer = 60;
    private float time;
    public float fireRate = 0.8f;
    public bool imortal { get; set; }
    public string stateName;
    public UIBoss uiBoss;
    public Collider2D colliderBoss;
    public AreaEnermy areaEnermy;

    [Header("Animation")]
    [SerializeField] public Animator animator;
    [SerializeField] public Boss1AniControl boss1AniControl;
    public bool isFacing;
    public bool isFacingRight;


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
    [HideInInspector]
    public AtkCloseB1FSM atkCloseState;

    private void Awake()
    {        
        checkDistanceState = new CheckDistanceB1FSM(this);
        idleState = new IdleB1FSM(this);
        normalAState = new NormalAB1FSM(this);
        dashAState = new DashAB1FSM(this);
        rangeAState = new RangeAB1Fsm(this);
        overdriveChangState = new OverdriveChangFSM(this);
        atkCloseState = new AtkCloseB1FSM(this);
        isFacing = true;
        jumpCenter = GameObject.Find("MIddleBossPoint").transform;
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

    public void Setdamage(int damage)
    {
        foreach (var item in lasers)
        {
            LaserFire laser = item.GetComponent<LaserFire>();
            laser.dmg = damage;
        }
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
            SetupHandGun();
            LaserFire laser = laserGun.GetComponent<LaserFire>();
            laser.speedRot = speedRot*10;
            laser.targetPlayer = target;
            laser.followCode = 2;
            await UniTask.WhenAll(laser.ShootLaser(charge, duration, speedMulti), laser.Aim(Atime));            
        }

    }

    public async UniTask ShootLaser(float charge, float duration, float speedMulti, float Atime ,float speedRot)
    {
        foreach (var laserGun in lasers)
        {
            SetupHandGun();
            LaserFire laser = laserGun.GetComponent<LaserFire>();
            laser.speedRot = speedRot*10;
            laser.targetPlayer = target;
            laser.followCode = 2;
            await UniTask.WhenAll(laser.ShootLaser(charge, duration, speedMulti), laser.Aim(Atime));
        }
    }

    public async UniTask ShootLaserFollowIn(float charge, float duration, float speedMulti, float Atime, float speedRot)
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
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot*10);
            handGun.eulerAngles = new Vector3(0, 0, newAngle);

            if (timer > time)
            {
                break;
            }

            await UniTask.Yield();
        }
    }

    public async UniTask ShootMissile(CancellationToken token)
    {
        Vector2 directionToPlayer = (ai.targetTransform.position - transform.position).normalized;
        float angleDirectionToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        float angleLeft = angleDirectionToPlayer + 90;
        float angleRight = angleDirectionToPlayer - 90;

        float spreadAngle = 10;
        int bulletCount = 3;
        float startAngle = -spreadAngle * ((bulletCount - 1) / 2.0f);
        await UniTask.WhenAll(Miissile(bulletCount, startAngle, spreadAngle, angleLeft, token), Miissile(bulletCount, startAngle, -spreadAngle, angleRight, token));
    }

    public async UniTask ShootMissile(int bulletCount, CancellationToken token)
    {
        Vector2 directionToPlayer = (ai.targetTransform.position - transform.position).normalized;
        float angleDirectionToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        float angleLeft = angleDirectionToPlayer + 90;
        float angleRight = angleDirectionToPlayer - 90;

        float spreadAngle = 10;
        float startAngle = -spreadAngle * ((bulletCount - 1) / 2.0f);

        await UniTask.WhenAll(
            Miissile(bulletCount, startAngle, spreadAngle, angleLeft, token),
            Miissile(bulletCount, startAngle, -spreadAngle, angleRight, token)
        );
    }

    public async UniTask Miissile(int bulletCount, float startAngle, float spreadAngle, float angleLR, CancellationToken token)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // ตรวจสอบการยกเลิก
            token.ThrowIfCancellationRequested();

            float angle = startAngle + (spreadAngle * i) + angleLR;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject bulletG = Instantiate(bulletmon, transform.position, rotation);
            bulletG.GetComponent<BulletFollow>().target = ai.targetTransform;

            await UniTask.WaitForSeconds(0.1f);
        }
    }

    public void ShootBladeslash()
    {
        Instantiate(bladeslash, bladeslashTransform.position, bladeslashTransform.rotation);
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        overdriveGage += damage;
        if (overdriveGage >= overdriveGageMax && !overdriveChang)
        {
            overdriveChang = true;
            ChangState(overdriveChangState);
        }
        Health -= damage;
        spriteFlash.Flash();    
        switch (type)
        {
            case DamageType.Rang:
                //lootDrop.InstantiateLoot(3);
                break;
            case DamageType.Melee:
                lootDrop.InstantiateLoot(dropChange,3);
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
        Instantiate(deadBody, gameObject.transform.position, Quaternion.identity).GetComponent<Animator>().SetFloat("isRight", isFacingRight ? 1 : -1);
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public void JumpCenter()
    {
        transform.position = jumpCenter.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (handGun.right).normalized * 5);
    }

    public void DiractionAttack()
    {
        if (isFacing)
        {
            Vector2 dir = (target.position - gameObject.transform.position).normalized;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            isFacingRight = targetAngle > -90 && targetAngle < 90;
            animator.SetBool("isRight", isFacingRight);
            if (isFacingRight)
            {
                animator.SetFloat("horizon", 1);
            }
            else
            {
                animator.SetFloat("horizon", -1);
            }
        }
        else
        {
            animator.SetFloat("horizon", isFacingRight ? 1 : -1);
        }

        if (rb.velocity != Vector2.zero /*&& !ai.endMove*/)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
    }

    public override List<BaseState> GetAllState()
    {
        List<BaseState> allState = new List<BaseState>();
        allState.Add(normalAState);
        allState.Add(dashAState);
        allState.Add(rangeAState);
        allState.Add(atkCloseState);
        return allState;
    }
}
