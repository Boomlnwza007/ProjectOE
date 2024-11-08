using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable , IEnergy
{
    [Header("Status")]
    public int health;
    private bool canHealth = true;
    public int maxHealth = 10;
    public int healUseEnergy = 5;
    public int energy { get; set; }
    public int maxEnergy { get; set; }
    public bool imortal { get; set; }
    public int ultimateEnergy { get; set; }
    public int maxUltimateEnergy { get; set; }
    public bool canGetUltimateEnergy { get; set; }

    [Header("CoolDown")]
    public float collDownHealth;

    [Header("Animetion")]
    private SpriteFlash spriteFlash;

    private void Awake()
    {
        imortal = false;
        //health = 20;
        maxEnergy = 10;
        maxUltimateEnergy = 10;
        ultimateEnergy = 0;
        energy = 0;
        canGetUltimateEnergy = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteFlash = GetComponent<SpriteFlash>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Heal") && health != maxHealth && energy >= healUseEnergy && canHealth)
        {
            energy -= healUseEnergy;
            health = maxHealth;
            canHealth = false;
            StartCoroutine(ColDownHealth(collDownHealth));
        }
    }

    public void Die()
    {
        PlayerControl.control.isdaed = true;
        PlayerControl.control.ShowGameOver();
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
            health -= damage;
            Debug.Log(health+" "+ damage);
            if (health <= 0)
            {
                health = 0;
                Die();
            }

            StartCoroutine(Imortal(0.1f));

        }

        if (spriteFlash != null)
        {
            spriteFlash.Flash();
        }
    }

    public void Heal()
    {
        health = maxHealth;
    }

    private IEnumerator ColDownHealth (float time)
    {
        yield return new WaitForSeconds(time);
        canHealth = true;
    }
}
