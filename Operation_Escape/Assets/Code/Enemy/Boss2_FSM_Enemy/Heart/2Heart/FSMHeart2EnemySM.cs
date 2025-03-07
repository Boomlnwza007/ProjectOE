using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart2EnemySM : FSMBaseBoss2EnemySM ,IDamageable
{
    [Header("Shield")]
    public GuardShield shield;

    [Header("Cooldown")]
    public float timePreSpike;
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    public Transform Zone;
    public GameObject particleZone;

    public BaseAnimEnemy animator;
    public bool imortal { get; set; }

    [HideInInspector]
    public H2IdleFSM Idle;
    [HideInInspector]
    public H2AttackFSM attack;
    [HideInInspector]
    public H2SummonFSM summon;

    private void Awake()
    {
        ResetPositionsMInion();
        Idle = new H2IdleFSM(this);
        attack = new H2AttackFSM(this);
        summon = new H2SummonFSM(this);
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

    public void Die()
    {
        ClearObj();
        Destroy(gameObject);
        areaEnermy?.Die(this);
        SpawnGun(); 
        Instantiate(deadBody, gameObject.transform.position, Quaternion.identity);
    }

    public override void ClearObj()
    {
        BeforDie();
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
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

            Instantiate(particleZone, spawnPosition, Quaternion.identity,Zone);
        }
    }

    public override List<BaseState> GetAllState()
    {
        List<BaseState> allState = new List<BaseState>();
        allState.Add(attack);
        allState.Add(summon);
        return allState;
    }
}
