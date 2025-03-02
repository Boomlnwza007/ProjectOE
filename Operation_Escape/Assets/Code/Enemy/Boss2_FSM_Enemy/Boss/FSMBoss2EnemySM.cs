using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBoss2EnemySM : FSMBaseBoss2EnemySM, IDamageable
{
    [Header("status")]
    public Boss2Mark areaMark;
    public Collider2D colliderBoss;
    public SpriteRenderer spriteBoss;
    public bool inRoom;
    public Boss2_Animation animator;
    public float speedEat;

    [Header("Prefab")]
    public GameObject eggMinion;
    public GameObject particleZone;

    [Header("Laser")]
    public GameObject laser;
    public float sizeLaser=5;
    public int cols = 20;
    public int rows = 40;
    public float spacingCols = 2f;
    public float spacingRows = 1f;
    public static float minionHave;

    [HideInInspector] 
    public IdleB2FSM idle;
    [HideInInspector]
    public StrikeB2FSM strike;
    [HideInInspector]
    public AreaAttackB2FSM area;
    [HideInInspector]
    public SwarmB2FSM swarm;
    [HideInInspector]
    public EatB2FSM eat;
    [HideInInspector]
    public CheckNextB2FSM checkNext;
    [HideInInspector]
    public LaserB2FSM laserState;

     public bool imortal { get; set; }

    private void Awake()
    {
        idle = new IdleB2FSM(this);
        strike = new StrikeB2FSM(this);
        area = new AreaAttackB2FSM(this);
        swarm = new SwarmB2FSM(this);
        eat = new EatB2FSM(this);
        checkNext = new CheckNextB2FSM(this);
        laserState = new LaserB2FSM(this);
        areaMark = GameObject.Find("Boss2Mark").GetComponent<Boss2Mark>();
        areaMark.state = this;
        grid = areaMark.grid;
        spawnPoint = areaMark.monSpawn;
        jumpCenter = areaMark.jumpCenter;
    }

    protected override BaseState GetInitialState()
    {
        return idle;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Health -= damage;
        spriteFlash?.Flash();
        switch (type)
        {
            case DamageType.Rang:
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
        //Instantiate(gunDrop, gameObject.transform.position, Quaternion.identity);
        if (areaEnermy != null)
        {
            areaEnermy.Die(this);
        }
        Destroy(gameObject);
        SpawnGun();
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public void SpawnParticle(float radius)
    {
        ZoneH2.hit = false;
        int numObjects = Mathf.FloorToInt((2 * Mathf.PI * radius) / 1); ;
        for (int i = 0; i < numObjects; i++)
        {
            float angle = i * Mathf.PI * 2f / numObjects; // คำนวณมุมของแต่ละจุด
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 spawnPosition = new Vector3(x, y, 0) + transform.position;

            Instantiate(particleZone, spawnPosition, Quaternion.identity);
        }
    }

    public void Jump(Vector3 position)
    {
        transform.position = position;
    }

    public void SpawnEgg()
    {
        for (int i = 0; i < spawnPoint.Count; i++)
        {
            Instantiate(eggMinion, spawnPoint[i].position, Quaternion.identity);
        }

    }

    public void SpawnLaserGrid()
    {
        Vector3 startPos = areaMark.startLaser.position;
        SpawnLaserCols(startPos);
        SpawnLaserRows(startPos);
    }

    public void SpawnLaserCols(float offset)
    {
        Vector3 startPos = areaMark.startLaser.position;
        startPos.x += offset;
        SpawnLaserCols(startPos);
    }

    public void SpawnLaserCols(Vector3 startPos)
    {
        for (int i = 1; i < cols; i++)
        {
            Vector3 spawnPos = startPos + new Vector3(i * (spacingCols + sizeLaser), 0, 0);
            Instantiate(laser, spawnPos, Quaternion.identity).GetComponent<LaserBoss2>().isUp = true;
        }
    }

    public void SpawnLaserCols(Vector3 startPos , float offset)
    {
        startPos.x += offset;
        SpawnLaserCols(startPos);
    }

    public void SpawnLaserRows(float offset)
    {
        Vector3 startPos = areaMark.startLaser.position;
        startPos.y += offset;
        SpawnLaserRows(startPos);
    }

    public void SpawnLaserRows(Vector3 startPos)
    {
        for (int j = 1; j < rows; j++)
        {
            Vector3 spawnPos = startPos + new Vector3(0, -j * (spacingRows + sizeLaser), 0);
            Instantiate(laser, spawnPos, Quaternion.identity).GetComponent<LaserBoss2>().isUp = false;
        }
    }

    public void SpawnLaserRows(Vector3 startPos, float offset)
    {
        startPos.y += offset;
        SpawnLaserRows(startPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                target.Takedamage(dmg, DamageType.Rang,10);
            }
        }
    }

    public override List<BaseState> GetAllState()
    {
        List<BaseState> allState = new List<BaseState>();
        allState.Add(strike);
        allState.Add(area);
        allState.Add(swarm);
        allState.Add(eat);
        allState.Add(laserState);
        return allState;
    }
}
