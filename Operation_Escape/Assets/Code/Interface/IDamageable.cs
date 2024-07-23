using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Takedamage(int damage);
    int Health { get; }
    void Die();
}
