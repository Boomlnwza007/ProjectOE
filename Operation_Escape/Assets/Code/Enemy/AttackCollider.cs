using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private StateMachine mon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable player = collision.GetComponent<IDamageable>();
        if (collision.CompareTag("Player"))
        {
            player.Takedamage(mon.dmg, DamageType.Melee, 0);
        }

    }
}
