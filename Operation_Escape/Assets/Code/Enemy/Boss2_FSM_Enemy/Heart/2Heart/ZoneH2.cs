using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneH2 : MonoBehaviour
{
    [SerializeField] private StateMachine mon;
    public int dmg;
    public static bool hit;
    public float size = 5;

    private void Start()
    {
        dmg = mon?.dmg ?? dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&& !hit)
        {
            hit = true;
            IDamageable player = collision.GetComponent<IDamageable>();
            player.Takedamage(dmg, DamageType.Melee, 0);
        }
    }   
}
