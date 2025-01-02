using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private StateMachine mon;
    public int dmg;
    public SpriteRenderer spriteRenderer;
    public Collider2D colliderA;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliderA = GetComponent<Collider2D>();
        dmg = mon?.dmg ?? dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamageable player = collision.GetComponent<IDamageable>();
            player.Takedamage(dmg, DamageType.Melee, 0);
        }

    }

    public void Re()
    {
        spriteRenderer.enabled = false;
        colliderA.enabled = false;
    }
}
