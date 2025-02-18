using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeZone : MonoBehaviour
{
    public int dmg;
    public float dps;
    private float time;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Hit(collision);
    }

    public void Hit(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            time += Time.deltaTime;
            if (time > dps)
            {
                if (collision.TryGetComponent(out IDamageable damage))
                {
                    damage.Takedamage(dmg, DamageType.Melee, 0);
                    SpikeZ.hit = true;
                }
                time = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            time = 0;
        }
    }
}
