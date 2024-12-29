using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMinion2EnemySM : StateMachine , IDamageable
{
    BaseAnimEnemy animator;
    public Vector2Int size;
    private Vector2Int gridPos;
    private GridBoss2 grid;

    [Header("Range")]
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    public Transform bulletTranform;

    [HideInInspector]
    public M2AttackFSM attack;
    [HideInInspector]
    public M2IdleFSM idle;

    public bool imortal { get; set; }

    private void Awake()
    {
        attack = new M2AttackFSM(this);
        idle = new M2IdleFSM(this);
    }

    public void Fire()
    {
        GameObject bulletG = Instantiate(bullet, bulletTranform.position, bulletTranform.rotation);
        bulletG.GetComponent<BulletFollow>().target = ai.targetTransform;
    }
    public void Setup(GridBoss2 grid, Vector2Int gridPos)
    {
        this.grid = grid;
        this.gridPos = gridPos;
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
        throw new System.NotImplementedException();
    }
}
