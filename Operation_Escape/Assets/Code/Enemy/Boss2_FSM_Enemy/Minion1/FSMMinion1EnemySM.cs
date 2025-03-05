using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMinion1EnemySM : StateMachine , IDamageable
{
    public M1_Animation animator;

    [Header("Charge")]
    public LayerMask raycastMaskWay;
    public LayerMask raycastMask;
    public float jumpLength = 6;
    public GameObject eDead;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D col;

    [HideInInspector]
    public M1AttackFSM attack;
    [HideInInspector]
    public M1IdleFSM idle;
    [HideInInspector]
    public M1checkDistanceFSM checkDistance;

    public string nameState;

    public bool imortal { get; set; }

    private void Awake()
    {
        attack = new M1AttackFSM(this);
        idle = new M1IdleFSM(this);
        checkDistance = new M1checkDistanceFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return idle;
    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
            nameState = curState.nameState;

        }
    }

    public void Run(float multiply)
    {
        ai.maxspeed *= multiply;
        //animator.animator.speed *= multiply;
    }

    public void Walk()
    {
        ai.maxspeed = Speed;
        //animator.animator.speed = 1;
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
                lootDrop?.InstantiateLoot(dropChange);
                break;
        }
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        FSMBoss2EnemySM.minionHave.Remove(this);
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(ai.position, 2f, raycastMask);
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out IDamageable dam) && hit.CompareTag("Player"))
            {
                dam.Takedamage(dmg, DamageType.Melee, 5);
            }
        }
        Instantiate(eDead,transform.position,Quaternion.identity);
        Die();
    }

    public void Jump(bool on)
    {
        if (on)
        {
            sprite.sortingLayerName = "TileMapON";

        }
        else
        {
            sprite.sortingLayerName = "Player";
        }
        col.enabled = on;
    }
}
