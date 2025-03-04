using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMinion2EnemySM : StateMachine , IDamageable
{
    public Animator animator;
    public Vector2Int size;
    private Vector2Int gridPos;
    [SerializeField]private GridBoss2 grid;
    public static List<FSMMinion2EnemySM> monInMap = new List<FSMMinion2EnemySM>();

    [Header("Range")]
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    public Transform bulletTranform;

    [Header("Range")]
    public float minShoot = 1;
    public float maxShoot = 3;

    [HideInInspector]
    public M2AttackFSM attack;
    [HideInInspector]
    public M2IdleFSM idle;

    public bool imortal { get; set; }

    private void Awake()
    {
        attack = new M2AttackFSM(this);
        idle = new M2IdleFSM(this);
        monInMap.Add(this);
    }

    protected override BaseState GetInitialState()
    {
        return idle;
    }

    public void Fire()
    {
        GameObject bulletG = Instantiate(bullet, bulletTranform.position, Quaternion.identity);
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
        monInMap.Remove(this);
        grid?.MarkGrid(gridPos, size, false);
        Destroy(gameObject.transform.parent.gameObject);
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
    }
}
