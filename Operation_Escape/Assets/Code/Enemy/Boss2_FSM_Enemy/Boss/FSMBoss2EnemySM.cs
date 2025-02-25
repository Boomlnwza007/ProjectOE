using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBoss2EnemySM : FSMBaseBoss2EnemySM, IDamageable
{
    [Header("status")]
    public Boss2Mark areaMark;
    public Collider2D colliderBoss;
    public bool inRoom;
    public Boss2_Animation animator;

    [Header("Prefab")]
    public GameObject eggMinion;
    public GameObject particleZone;

    public static float minionHave;

    [HideInInspector]
    public AreaEnermy areaEnermy;
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
    public LaserB2FSM laser;

     public bool imortal { get; set; }

    private void Awake()
    {
        idle = new IdleB2FSM(this);
        strike = new StrikeB2FSM(this);
        area = new AreaAttackB2FSM(this);
        swarm = new SwarmB2FSM(this);
        eat = new EatB2FSM(this);
        checkNext = new CheckNextB2FSM(this);
        laser = new LaserB2FSM(this);
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
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
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
}
