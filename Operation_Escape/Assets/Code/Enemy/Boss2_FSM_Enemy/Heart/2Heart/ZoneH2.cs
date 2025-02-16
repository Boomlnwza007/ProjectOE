using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneH2 : MonoBehaviour
{
    [SerializeField] private StateMachine mon;
    public int dmg;
    public float distance;
    private float size = 10;

    private void Start()
    {
        dmg = mon?.dmg ?? dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Vector2.Distance(transform.position, collision.transform.position) > distance - size)
            {
                IDamageable player = collision.GetComponent<IDamageable>();
                player.Takedamage(dmg, DamageType.Melee, 0);
            }
        }
    }   
}
