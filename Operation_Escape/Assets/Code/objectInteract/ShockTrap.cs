using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTrap : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    private Collider2D trap;
    private float time;
    private bool isRunning = false;
    public float timeBetween;
    public float duration;
    public int damage = 60;
    public float dpsDamage = 1;
    private bool canDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning && time >= timeBetween)
        {
            trap = Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Collider2D>();
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
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    public void Shock()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Physics2D.OverlapCollider(trap, filter, colliders);
        foreach (var hit in colliders)
        {
            IDamageable any = hit.GetComponent<IDamageable>();
            if (any != null)
            {
                if (canDamage)
                {
                    canDamage = false;
                    any.Takedamage(damage, DamageType.Rang, 0);
                    DamageHit().Forget();
                }                
            }
        }
    }

    public async UniTask DamageHit()
    {
        await UniTask.WaitForSeconds(dpsDamage);
        canDamage = true;
        await UniTask.WaitForSeconds(dpsDamage);
    }
}
