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
    public float speedStrike;
    [SerializeField]public string curStateName;
    public bool phase;
    public bool isInGound;
    private DummyBoss2 dummy;
    [HideInInspector] public bool phaseStart;
    public HeartSound sound;

    [Header("Prefab")]
    public GameObject eggMinion;
    public GameObject particleZone;
    public GameObject particleLightning;
    public GameObject dummyBoss;

    [Header("Laser")]
    public GameObject laser;
    public float sizeLaser=5;
    public int cols = 20;
    public int rows = 40;
    public float spacingCols = 2f;
    public float spacingRows = 1f;
    public static List<StateMachine> minionHave = new List<StateMachine>();

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
    public LaserB2FSM laserState;
    [HideInInspector]
    public MergeB2FSM merge;
    [HideInInspector]
    public MstrikeB2FSM mStrike;
    [HideInInspector]
    public MareaB2FSM mArea;
    [HideInInspector]
    public MswarmB2FSM mSwarm;
    [HideInInspector]
    public CheckNextB2FSM checkNext;


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
        merge = new MergeB2FSM(this);
        mStrike = new MstrikeB2FSM(this);
        mArea = new MareaB2FSM(this);
        mSwarm = new MswarmB2FSM(this);
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

        if (!phase && Health <= maxHealth/2 )
        {
            phaseStart = true;            
        }

    }

    public void Die()
    {
        //Instantiate(gunDrop, gameObject.transform.position, Quaternion.identity);
        if (areaEnermy != null)
        {
            areaEnermy.Die(this);
        }
        dummy.Die();
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
        SpawnEgg(spawnPoint.Count);
    }

    public void SpawnEgg(int count)
    {
        SpawnEggP2(count, 3);
    }

    public void SpawnEggPos(int pos)
    {
        EggBoss2 egg = Instantiate(eggMinion, spawnPoint[pos].position, Quaternion.identity).GetComponent<EggBoss2>();
        egg.minionID = 3;
    }

    public void SpawnEggP2(int count , int id)
    {
        count = Mathf.Clamp(count, 0, spawnPoint.Count);
        List<Transform> availableSpawns = new List<Transform>(spawnPoint);
        if (count >= availableSpawns.Count)
        {
            count = availableSpawns.Count;
        }

        for (int i = 0; i < availableSpawns.Count - count; i++)
        {
            availableSpawns.RemoveAt(Random.Range(0, availableSpawns.Count));
        }

        foreach (var spawn in availableSpawns)
        {
            EggBoss2 egg = Instantiate(eggMinion, spawn.position, Quaternion.identity).GetComponent<EggBoss2>();
            egg.minionID = id;
        }
    }

    public void SpawnEggP2()
    {
        List<int> spawnIndices = new List<int> { 0, 1, 2 };

        int minion1Index = spawnIndices[Random.Range(0, spawnIndices.Count)];
        spawnIndices.Remove(minion1Index);

        int minion2Index = spawnIndices[Random.Range(0, spawnIndices.Count)];
        spawnIndices.Remove(minion2Index);

        for (int i = 0; i < spawnPoint.Count; i++)
        {
            EggBoss2 egg = Instantiate(eggMinion, spawnPoint[i].position, Quaternion.identity).GetComponent<EggBoss2>();

            if (i == minion1Index)
                egg.minionID = 1;
            else if (i == minion2Index)
                egg.minionID = 2;
            else
                egg.minionID = 3;
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
            Instantiate(laser, spawnPos, Quaternion.identity, transform).GetComponent<LaserBoss2>().isUp = true;
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
            Instantiate(laser, spawnPos, Quaternion.identity,transform).GetComponent<LaserBoss2>().isUp = false;
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
                target.Takedamage(dmg, DamageType.Melee,10);
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
        allState.Add(merge);
        allState.Add(mStrike);
        allState.Add(mArea);
        allState.Add(mSwarm);
        return allState;
    }
    public void CabWalk(int on)
    {
        if (on == 0)
        {
            ai.canMove = false;

        }
        else
        {
            ai.canMove = true;

        }
    }

    public void GoundCheck(int n)
    {
        if (n==0)
        {
            isInGound = false;
        }
        else
        {
            isInGound = true;
        }
    }

    public DummyBoss2 SpawnDummy()
    {
        Vector2 dropPosition2;
        do
        {
            dropPosition2 = (Vector2)transform.position + Random.insideUnitCircle * 8f;
        } while (Vector2.Distance(transform.position, dropPosition2) < 6f);
        dummy = Instantiate(dummyBoss, dropPosition2, Quaternion.identity).GetComponent<DummyBoss2>();
        return dummy;
    }

    public void SpawnLightning()
    {
        SpawnLightning(ai.targetTransform.position);
    }

    public void SpawnLightning(Vector2 pos)
    {
        Instantiate(particleLightning, pos, Quaternion.identity).GetComponent<Lightning>().move = true;
    }
}
