using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DummyBoss2 : MonoBehaviour, IDamageable
{
    public FSMBoss2EnemySM boss2;
    public Boss2_Animation ani;
    public LootTable lootDrop;
    public GameObject gun;
    public SpriteFlash spriteFlash;
    private CancellationTokenSource cancellationToken;
    public Rigidbody2D rb;
    public Collider2D colliderBoss;
    public bool imortal { get; set; }
    public bool agian;
    public bool inRoom;
    public HeartSound sound;

    private void Start()
    {
        //boss2 = GameObject.Find("Boss2").GetComponent<FSMBoss2EnemySM>();
        //StrikeB2FSM().Forget();
    }

    public async UniTask StrikeB2FSM()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;
        var state = boss2;
        //await UniTask.WaitForSeconds(1f, cancellationToken: token);

        try
        {
            sound.PlayPreAtk(0);
            ani.ChangeAnimationAttack("Strike");

            await UniTask.WaitUntil(() => ani.endAnim, cancellationToken: token);
            await Strike();
            RandomEdge();
            //for (int i = 0; i < 3; i++)
            //{
            //    await StrikeAtk();
            //}
            rb.velocity = Vector2.zero;
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }

    }

    public async UniTask StrikeAtk()
    {
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;

        try
        {
            agian = true;
            await Strike();
            RandomEdge();
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Attack was cancelled.");
            return;
        }
    }

    public async UniTask Strike()
    {
        var state = boss2;
        Vector2 dir = (state.ai.targetTransform.position - transform.position).normalized;
        colliderBoss.isTrigger = true;
        if (dir.x < 0)
        {
            ani.ChangeAnimationAttack("Striking_L");
        }
        else
        {
            ani.ChangeAnimationAttack("Striking_R");
        }

        sound.PlayMonAtk(0);


        float time = 0;        
        while (inRoom || agian)
        {
            rb.velocity = dir * state.speedStrike;
            if (inRoom)
            {
                agian = false;
            }
            time += Time.deltaTime;

            if (time > 3)
            {
                dir = (state.ai.targetTransform.position - transform.position).normalized;
                if (dir.x < 0)
                {
                    ani.ChangeAnimationAttack("Striking_L");
                }
                else
                {
                    ani.ChangeAnimationAttack("Striking_R");
                }
                time = 0;
            }

            await UniTask.Yield(cancellationToken: cancellationToken.Token);
        }
    }

    public void RandomEdge()
    {
        var state = boss2;
        int rEdge = Random.Range(0, 4);
        int rNumber = 0;
        rb.velocity = Vector2.zero;
        Vector3 pos;
        switch (rEdge)
        {
            case 0:
                rNumber = Random.Range(0, state.areaMark.top.Length);
                pos = state.areaMark.top[rNumber].position;
                pos.x += Random.Range(-5, 5);
                transform.position = pos;

                break;
            case 1:
                rNumber = Random.Range(0, state.areaMark.down.Length);
                pos = state.areaMark.down[rNumber].position;
                pos.x += Random.Range(-5, 5);
                transform.position = pos;

                break;
            case 2:
                rNumber = Random.Range(0, state.areaMark.left.Length);
                pos = state.areaMark.left[rNumber].position;
                pos.y += Random.Range(-5, 5);
                transform.position = pos;

                break;
            case 3:
                rNumber = Random.Range(0, state.areaMark.right.Length);
                pos = state.areaMark.left[rNumber].position;
                pos.y += Random.Range(-5, 5);
                transform.position = pos;

                break;
        }
    }

    public void Die()
    {
        cancellationToken.Cancel();
        Destroy(gameObject);
        if (gun != null)
        {
            Instantiate(gun, transform.position, Quaternion.identity);
        }
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        boss2.Health -= damage;
        spriteFlash?.Flash();
        switch (type)
        {
            case DamageType.Rang:
                break;
            case DamageType.Melee:
                lootDrop.InstantiateLoot(0);
                break;
        }
        if (boss2.Health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                target.Takedamage(boss2.dmg, DamageType.Melee, 10);
            }
        }
    }
}
