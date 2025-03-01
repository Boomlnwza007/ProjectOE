using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnergyBarrel : MonoBehaviour, IObjInteract , IRestartOBJ
{
    [SerializeField] private GameObject hitbox;
    private GameObject bomb;
    private Coroutine fadeBomb;
    public int damage = 10;
    public float timeBlast = 2f;
    public bool blast;
    public LootTable lootDrop;
    public Animator animator;
    public GameObject[] objAll; 

    [Header("------ Audio Base ------")]
    public AudioSource sfxSource;
    public AudioClip PreExplode;
    public AudioClip explode;
    public AudioClip meleeHit;

    public void Interact(DamageType type)
    {
        switch (type)
        {
            case DamageType.Rang:
                if (!blast)
                {
                    SetBomb();
                    fadeBomb = StartCoroutine(FadeBomb(timeBlast));
                }
                else
                {
                    StopCoroutine(fadeBomb);
                    BombBlast().Forget();
                }
                break;
            case DamageType.Melee:
                if (!blast)
                {
                    MeleeBomb().Forget();
                }
                else
                {
                    StopCoroutine(fadeBomb);
                    BombBlast().Forget();
                }                
                break;
            default:
                break;
        }
        
    }

    private async UniTask MeleeBomb()
    {
        lootDrop.InstantiateLoot(0);
        var sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        sfxSource.PlayOneShot(meleeHit);
        await UniTask.WaitForSeconds(meleeHit.length);
        sprite.enabled = true;
        gameObject.SetActive(false);
    }

    public void SetBomb()
    {
        animator.SetTrigger("Bomb");
        bomb = Instantiate(hitbox, transform.position, Quaternion.identity);
        blast = true;
    }

    public void MakeDamage()
    {
        bomb.GetComponentInChildren<ParticleSystem>().Play();
        Collider2D bombCollider = bomb.GetComponent<Collider2D>();
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Physics2D.OverlapCollider(bombCollider, filter, colliders);
        foreach (var hit in colliders)
        {
            IDamageable any = hit.GetComponent<IDamageable>();
            if (any != null)
            {
                any.Takedamage(damage, DamageType.Rang, 0);
            }
        }
    }

    public IEnumerator FadeBomb(float duration)
    {        
        yield return new WaitForSeconds(duration);
        BombBlast().Forget();
    }

    public async UniTask BombBlast()
    {
        SetHide(false);
        MakeDamage();
        sfxSource.PlayOneShot(explode);
        await UniTask.WaitForSeconds(explode.length);
        SetHide(true);
        gameObject.SetActive(false);
    }

    public void SetHide(bool hide)
    {
        GetComponent<SpriteRenderer>().enabled = hide;
        GetComponent<Collider2D>().enabled = hide;
        foreach (var item in objAll)

        {
            item.SetActive(hide);
        }
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        if (bomb != null)
        {
            Destroy(bomb.gameObject);
        }
        blast = false;
    }
}
