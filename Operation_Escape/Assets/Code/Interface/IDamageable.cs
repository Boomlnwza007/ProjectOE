using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType 
{ 
    Rang,Melee,
}
public interface IDamageable
{
    public void Takedamage(int damage, DamageType type ,float knockBack);
    public void Die();
    public IEnumerator Imortal(float wait);
    public bool imortal { get; set; }
}
