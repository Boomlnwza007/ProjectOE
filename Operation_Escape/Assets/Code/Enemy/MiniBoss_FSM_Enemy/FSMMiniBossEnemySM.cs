using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMiniBossEnemySM : StateMachine, IDamageable
{
    [Header("status")]
    public bool cooldown;
    public float time;
    public float timeCooldown = 6f;
    public bool imortal { get; set; }
    public AreaEnermy areaEnermy;
    public string stateName;

    private void Awake()
    {
        

    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
            stateName = curState.nameState;
        }
        if (cooldown)
        {
            time += Time.deltaTime;
            if (time > timeCooldown)
            {
                time = 0;
                cooldown = false;
            }
        }
    }

    //protected override BaseState GetInitialState()
    //{
    //    return wanderState;
    //}

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Health -= damage;
        switch (type)
        {
            case DamageType.Rang:
                //lootDrop.InstantiateLoot(3);
                break;
            case DamageType.Melee:
                lootDrop.InstantiateLoot(1);
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

    public override void CombatPhaseOn()
    {
        //ChangState(CheckDistance);
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
    }
}
