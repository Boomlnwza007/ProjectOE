using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class BulletSticky : BaseBullet
{
    public int exDmg = 5;
    public float timeBomb = 10;
    public SpriteRenderer spriteBullet;
    private IDamageable targetDmg;
    private StateMachine enermy;
    private bool sticky;
    [SerializeField]private SpriteRenderer spriteR;
    [SerializeField]private Sprite spriteSticky;
    private CancellationTokenSource cancellationTokenSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ready = true;
        rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        if (spriteR != null)
        {
            if (spriteR.enabled && !spriteBullet.enabled)
            {
                spriteBullet.enabled = true;
                foreach (Transform item in gameObject.transform)
                {
                    item.gameObject.SetActive(true);
                }
            }
            if (!spriteR.enabled && spriteBullet.enabled)
            {
                spriteBullet.enabled = false;
                foreach (Transform item in gameObject.transform)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IObjInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);
            Destroy(gameObject);
            Expo();
        }
        else if (collision.tag == tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                targetDmg = target;
                targetDmg.Takedamage(damage, DamageType.Rang, knockbackForce);
                gameObject.transform.parent = collision.transform;
                rb.velocity = Vector3.zero;  
                Destroy(rb);
                Destroy(GetComponent<Collider2D>());
                spriteBullet.sprite = spriteSticky;
                spriteR = collision.GetComponent<SpriteRenderer>() ?? collision.GetComponentInChildren<SpriteRenderer>();
                if (collision.TryGetComponent(out enermy))
                {
                    enermy.dropChange++;
                }
                StartBlast();
            }
        }        
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
            Expo();
        }
    }

    public async UniTask Blast(CancellationToken cancellationToken)
    {
        float timer = 0f;
        float flashSpeed = 1f;
        Color originalColor = spriteBullet.color;
        sticky = true;

        while (timer < timeBomb)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            timer += Time.deltaTime;
            
            float redValue = Mathf.Lerp(0, 1, timer / timeBomb);
            spriteBullet.color = new Color(redValue, originalColor.g * (1 - redValue), originalColor.b * (1 - redValue));
            
            if (timer >= timeBomb - 3f)
            {
                flashSpeed = Mathf.Lerp(1f, 10f, (timer - (timeBomb - 3f)) / 3f);
                spriteBullet.color = Color.Lerp(originalColor, Color.red, Mathf.PingPong(Time.time * flashSpeed, 1f));
            }

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        if (enermy != null)
        {
            enermy.dropChange--;
            if (ultimate)
            {
                enermy.lootDrop.InstantiateLoot(1);
            }
        }

        targetDmg.Takedamage(exDmg, DamageType.Rang, knockbackForce);
        Destroy(gameObject);
        Expo();
    }

    public void StartBlast()
    {
        cancellationTokenSource = new CancellationTokenSource();
        Blast(cancellationTokenSource.Token).Forget();
    }

    public void StopBlast()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }

    private void OnBecameInvisible()
    {
        if (!sticky)
        {
            Destroy(gameObject);
            Expo();
        }
    }

    public void DelAFMelee()
    {
        StopBlast();
        if (enermy != null)
        {
            enermy.dropChange--;
        }
        Destroy(gameObject);
        Expo();
    }
}
