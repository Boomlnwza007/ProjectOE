using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable , IEnergy
{
    public int health;
    public int maxHealth = 10;
    public int energy { get; set; }
    public int maxEnergt { get; set; }
    public bool imortal { get; set; }
    private SpriteFlash spriteFlash;

    // Start is called before the first frame update
    void Start()
    {
        //combat = GetComponent<PlayerCombat>();
        health = maxHealth;
        maxEnergt = 10;
        energy = maxEnergt;
        spriteFlash = GetComponent<SpriteFlash>();
    }

    public void Die()
    {
        
    }

    public IEnumerator Imortal(float wait)
    {
        imortal = true;
        yield return new WaitForSeconds(wait);
        imortal = false;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        if (!imortal)
        {
            StartCoroutine(Imortal(0.1f));
            health -= damage;
            if (spriteFlash != null)
            {
                spriteFlash.Flash();
            }

            if (health <= 0)
            {
                Die();
            }
        }
    }

    public void Heal()
    {
        health = maxHealth;
    }
}
