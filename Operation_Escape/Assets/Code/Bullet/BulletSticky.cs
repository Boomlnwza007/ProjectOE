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
    private CancellationTokenSource cancellationTokenSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ready = true;
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                targetDmg = target;
                targetDmg.Takedamage(damage, DamageType.Rang, knockbackForce);
                gameObject.transform.parent = collision.transform;
                rb.velocity = Vector3.zero;  
                Destroy(rb);
                if (collision.TryGetComponent(out enermy))
                {
                    enermy.dropChange++;
                }
                StartBlast();
            }
        }
        else if (collision.TryGetComponent(out IBulletInteract bulletInteract))
        {
            bulletInteract.Interact(DamageType.Rang);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    public async UniTask Blast(CancellationToken cancellationToken)
    {
        float timer = 0f;
        float flashSpeed = 1f; // ความเร็วเริ่มต้นของการกระพริบ
        Color originalColor = spriteBullet.color; // เก็บสีเดิมไว้
        sticky = true;

        while (timer < timeBomb)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Blast was canceled!");
                return;
            }

            timer += Time.deltaTime;

            // เปลี่ยนสีให้ค่อย ๆ แดงเมื่อเข้าใกล้ 10 วินาที
            float redValue = Mathf.Lerp(0, 1, timer / timeBomb);
            spriteBullet.color = new Color(redValue, originalColor.g * (1 - redValue), originalColor.b * (1 - redValue));

            // หากเวลาเกิน 7 วินาที เริ่มกระพริบ
            if (timer >= timeBomb - 3f)
            {
                // เพิ่มความเร็วของการกระพริบเมื่อใกล้ถึงเวลา
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
                enermy.lootDrop.InstantiateLoot(4);
            }
        }

        targetDmg.Takedamage(exDmg, DamageType.Rang, knockbackForce);
        Destroy(gameObject);
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
        }
    }

    public void DelAFMelee()
    {
        StopBlast();
        enermy.dropChange--;
        Destroy(gameObject);
    }
}
