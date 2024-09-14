using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTrap : MonoBehaviour
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
                trap = Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Collider2D>();
                isRunning = true;
                time = 0f;
                GetComponent<SpriteRenderer>().color = Color.red;

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
                    Destroy(gameObject);
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
        trapOn = true;
    }
}
