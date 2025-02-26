using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart4EnemySM : FSMBaseBoss2EnemySM ,IDamageable
{
    [Header("Shield")]
    public GuardShield shield;

    [Header("Cooldown")]
    public float timePreSpike;
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    [Header("Range")]
    [SerializeField] public GameObject bulletmon;
    private List<GameObject> lasers = new List<GameObject>();
    public float speedRot = 10f;
    public GameObject lineRendererPrefab;
    public Transform handGun;
    public Transform bulletTransform;

    public BaseAnimEnemy animator;
    public bool imortal { get; set; }

    [HideInInspector]
    public H4IdleFSM Idle;
    [HideInInspector]
    public H4AttackFSM attack;
    [HideInInspector]
    public H4SummonFSM summon;

    private void Awake()
    {
        ResetPositionsMInion();
        Idle = new H4IdleFSM(this);
        attack = new H4AttackFSM(this);
        summon = new H4SummonFSM(this);
        spriteFlash = GetComponent<SpriteFlash>();

    }

    protected override BaseState GetInitialState()
    {
        return Idle;
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

    public void CreatLaserGun()
    {
        SetupHandGun();
        GameObject laser = Instantiate(lineRendererPrefab, transform.position, Quaternion.identity, handGun);
        LaserFire laserg = laser.GetComponent<LaserFire>();
        laserg.SetStartFollow(target.position);
        lasers.Add(laser);
    }

    public void SetupHandGun()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        handGun.localRotation = Quaternion.Euler(0, 0, targetAngle);
    }

    public async UniTask ShootLaser(float charge, float duration, float speedMulti, float Atime/*, float speedRot*/)
    {
        foreach (var laserGun in lasers)
        {
            LaserFire laser = laserGun.GetComponent<LaserFire>();
            laser.speedRot = speedRot*10;
            laser.targetPlayer = target;
            laser.followCode = 2;
            await UniTask.WhenAll(laser.ShootLaser(charge, duration, speedMulti), laser.Aim(Atime));
        }
    }

    public void DelLaserGun()
    {
        foreach (var gun in lasers)
        {
            Destroy(gun);
        }
        lasers.Clear();
    }

    public void ChargeBullet()
    {
        Instantiate(bulletmon, bulletTransform.position, Quaternion.identity);
    }


    public void Die()
    {
        Destroy(gameObject);
        areaEnermy?.Die(this);
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
    }
}
