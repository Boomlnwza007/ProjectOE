using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable , IEnergy
{
    [Header("Status")]
    public int health;
    public int maxHealth = 10;
    public int healUseEnergy = 5;
    public int energy { get; set; }
    public int maxEnergy { get; set; }
    public bool imortal { get; set; }
    public int ultimateEnergy { get; set; }
    public int maxUltimateEnergy { get; set; }
    public bool canGetUltimateEnergy { get; set; }

    [Header("Animetion")]
    private SpriteFlash spriteFlash;

    private void Awake()
    {
        health = maxHealth;
        maxEnergy = 10;
        maxUltimateEnergy = 10;
        ultimateEnergy = 0;
        energy = maxEnergy;
        canGetUltimateEnergy = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteFlash = GetComponent<SpriteFlash>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Heal") && health != maxHealth && energy >= healUseEnergy)
        {
            energy -= healUseEnergy;
            health = maxHealth;
        }
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
