using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BulletSticky : BaseBullet
{
    public int exDmg = 5;
    public float timeBomb = 10;
    public SpriteRenderer spriteBullet;
    public bool ultimate;
    private IDamageable targetDmg;
    private StateMachine enermy;
    private bool sticky;

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
                Blast().Forget();
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

    public async UniTask Blast()
    {
        float timer = 0f;
        float flashSpeed = 1f; // ��������������鹢ͧ��á�о�Ժ
        Color originalColor = spriteBullet.color; // ����������
        sticky = true;
        while (timer < timeBomb)
        {
            timer += Time.deltaTime;
            // ����¹�������� � ᴧ����������� 10 �Թҷ�
            float redValue = Mathf.Lerp(0, 1, timer / timeBomb);
            spriteBullet.color = new Color(redValue, originalColor.g * (1 - redValue), originalColor.b * (1 - redValue));

            // �ҡ�����Թ 7 �Թҷ� �������о�Ժ
            if (timer >= timeBomb - 3f)
            {
                // �����������Ǣͧ��á�о�Ժ��������֧����
                flashSpeed = Mathf.Lerp(1f, 10f, (timer - (timeBomb - 3f)) / 3f);
                spriteBullet.color = Color.Lerp(originalColor, Color.red, Mathf.PingPong(Time.time * flashSpeed, 1f));
            }

            await UniTask.Yield();
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

    private void OnBecameInvisible()
    {
        if (!sticky)
        {
            Destroy(gameObject);
        }
    }
}