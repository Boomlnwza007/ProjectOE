using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGun_Shield : MonoBehaviour,IDamageable
{
    public bool imortal { get; set; }
    public int maxhp;
    public SpriteRenderer shield;
    public Collider2D colliderShield;
    public float timeUseN = 3;
    public float timeUseExZero = 10;
    public float timeDurationUltimate = 10;
    public SpriteFlash flash;
    private float time;
    private bool timeDmg;
    private float timer=0;

    private void Start()
    {
        PlayerControl.control.playerState.imortal = true;
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Imortal(float wait)
    {
        imortal = true;
        yield return new WaitForSeconds(timeDurationUltimate);
        imortal = false;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        if (!imortal)
        {
            if (ShieldGun.hpShield > 0)
            {
                ShieldGun.hpShield -= damage;
                flash.Flash();
                if (ShieldGun.hpShield <= 0)
                {
                    ShieldGun.hpShield = 0;
                    shield.enabled = false;
                    colliderShield.enabled = false;
                    timeDmg = true;
                    time = timeUseExZero;
                    timer = 0;
                }
                else
                {
                    timeDmg = true;
                    timer = 0;
                    time = timeUseN;
                }
            }
        }       
    }

    private void Update()
    {
        if (timeDmg)
        {
            PlayerControl.control.playerState.imortal = false;
            timer += Time.deltaTime;
            if (timer > time)
            {
                timeDmg = false;
                ShieldGun.hpShield = maxhp;
                shield.enabled = true;
                colliderShield.enabled = true;
                PlayerControl.control.playerState.imortal = true;

            }
        }       
    }

    public void ShieldOn(bool on)
    {
        if (!timeDmg)
        {
            PlayerControl.control.playerState.imortal = on;
            shield.enabled = on;
            colliderShield.enabled = on;
        }
        
    }
}
