using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTrap : MonoBehaviour , IRestartOBJ
{
    [SerializeField] private GameObject hitbox;
    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    private Collider2D trap;
    private float time;
    private bool isRunning = false;
    public float timeAc;
    public float duration;
    public int damage = 60;
    public float dpsDamage = 1;
    private bool trapOn;

    [Header("------ Audio Base ------")]
    public AudioSource sfxSource;
    public AudioClip prepare;
    public AudioClip explode;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (trapOn)
        {
            if (!isRunning && time >= timeAc)
            {                
                isRunning = true;
                time = 0f;
            }

            if (isRunning)
            {
                time += Time.deltaTime;
                Shock();
                if (time >= duration)
                {
                    isRunning = false;
                    time = 0f;
                    Destroy(trap.gameObject);                    
                    gameObject.SetActive(false);
                }
            }
            else
            {
                time += Time.deltaTime;
            }
        }        
    }

    public void Shock()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Physics2D.OverlapCollider(trap, filter, colliders);
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out IDamageable dam))
            {
                if (!hitTargets.Contains(dam))
                {
                    dam.Takedamage(damage, DamageType.Rang, 0);
                    hitTargets.Add(dam);
                    DamageHit().Forget();                    
                }
                
            }
        }
    }

    public async UniTask DamageHit()
    {
        await UniTask.WaitForSeconds(dpsDamage);
        hitTargets.Clear();
        await UniTask.WaitForSeconds(dpsDamage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!trapOn)
        {
            trapOn = true;
            trap = Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Collider2D>();
            StartCoroutine(FadeBomb(trap.GetComponent<SpriteRenderer>(), timeAc));
            sfxSource.PlayOneShot(prepare);
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

        color = Color.red;
        color.a = 1f;
        spriteRenderer.color = color;
        sfxSource.PlayOneShot(explode);
    }

    public void Reset()
    {
        trapOn = false;
        gameObject.SetActive(true);
        time = 0;

        if (trap != null)
        {
            Destroy(trap.gameObject);
        }
    }
}
