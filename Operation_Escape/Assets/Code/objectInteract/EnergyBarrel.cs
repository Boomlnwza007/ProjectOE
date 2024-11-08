using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarrel : MonoBehaviour, IBulletInteract , IRestartOBJ
{
    [SerializeField] private GameObject hitbox;
    private Collider2D bomb;
    public int damage = 10;
    public float timeBlast = 2f;
    public bool blast;
    public LootTable lootDrop;

    public void Interact(DamageType type)
    {
        switch (type)
        {
            case DamageType.Rang:
                if (!blast)
                {
                    SetBomb();
                    StartCoroutine(FadeBomb(bomb.GetComponent<SpriteRenderer>(), timeBlast));
                }
                else
                {
                    StopCoroutine("FadeBomb");
                    BombBlast(bomb.GetComponent<SpriteRenderer>());
                }
                break;
            case DamageType.Melee:
                if (!blast)
                {
                    lootDrop.InstantiateLoot(0);
                    gameObject.SetActive(false);
                }
                else
                {
                    StopCoroutine("FadeBomb");
                    BombBlast(bomb.GetComponent<SpriteRenderer>());
                }                
                break;
            default:
                break;
        }
        
    }

    public void SetBomb()
    {
        bomb = Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Collider2D>();
        blast = true;
    }

    public void MakeDamage()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Physics2D.OverlapCollider(bomb, filter, colliders);
        foreach (var hit in colliders)
        {
            IDamageable any = hit.GetComponent<IDamageable>();
            if (any != null)
            {
                any.Takedamage(damage, DamageType.Rang, 0);
            }
        }
    }

    public IEnumerator FadeBomb(SpriteRenderer spriteRenderer, float duration)
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            color.a = newAlpha;
            spriteRenderer.color = color;
            yield return null;
        }

        color = Color.white;
        color.a = 1f;
        spriteRenderer.color = color;
        BombBlast(spriteRenderer);
    }

    public void BombBlast(SpriteRenderer spriteRenderer)
    {
        Color color = Color.white;
        color.a = 1f;
        spriteRenderer.color = color;
        MakeDamage();
        gameObject.SetActive(false);
        Destroy(bomb.gameObject);
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
