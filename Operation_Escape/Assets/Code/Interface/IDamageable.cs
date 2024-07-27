using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType 
{ 
    Rang,Melee,
}
public interface IDamageable
{
    void Takedamage(int damage, DamageType type);    
    void Die();
}
