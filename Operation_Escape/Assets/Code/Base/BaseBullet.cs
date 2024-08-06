using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public int damage;
    public float force;
    public float knockBack;
    public string tagUse;
    public Transform target;
    public virtual void ResetGameObj() {}
}
