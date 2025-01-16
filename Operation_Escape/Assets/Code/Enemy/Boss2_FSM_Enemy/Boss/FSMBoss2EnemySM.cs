using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBoss2EnemySM : StateMachine, IDamageable
{
    [Header("status")]
    public AreaEnermy areaEnermy;

    public bool imortal { get; set; }

    private void Awake()
    {
        
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
}
