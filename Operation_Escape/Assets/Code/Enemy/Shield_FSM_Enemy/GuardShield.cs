using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardShield : MonoBehaviour
{
    public bool canGuard = true;
    [HideInInspector]public bool redy = true;
    public bool conShield = true;
    private SpriteRenderer shieldSprite;
    private Collider2D shieldCollider;
    private Transform target;
    public ContactFilter2D filter;
    public float cooldown;
    private float timeCooldown = 0;

    [Header("------ Audio Base ------")]
    public AudioSource audioGame;
    public AudioClip shieldBreak;
    public AudioClip shieldDeflec;
    public AudioClip shieldRecovery;
    public AudioClip shieldOn;
    public AudioClip shieldOff;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        shieldSprite = gameObject.GetComponent<SpriteRenderer>();
        shieldCollider = gameObject.GetComponent<Collider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (conShield)
        {
            shieldSprite.enabled = redy ? canGuard : false;
            shieldCollider.enabled = redy ? canGuard : false;

            if (canGuard)
            {
                //gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
                if (redy)
                {
                    Guard(angle);
                }
            }
            else
            {
                timeCooldown += Time.deltaTime;
                if (timeCooldown > cooldown)
                {
                    canGuard = true;
                    timeCooldown = 0;
                    audioGame.PlayOneShot(shieldRecovery);
                }
            }
        }        
    }

    public void Guard(float angle)
    {
        List<Collider2D> colliderHit = new List<Collider2D>();
        Physics2D.OverlapCollider(shieldCollider, filter, colliderHit);
        foreach (var collider in colliderHit)
        {
            if (collider == shieldCollider) continue;

            if (!collider.CompareTag("BulletPlayer")) continue;

            BaseBullet bullet = collider.GetComponent<BaseBullet>();
            if (bullet == null || bullet.tagUse == "Player" || !bullet.ready) continue;

            HandleBulletInteraction(bullet, angle);    
        }

        colliderHit.Clear();
    }

    public void HandleBulletInteraction(BaseBullet bullet, float angle)
    {
        if (bullet is BulletCharge)
        {
            BreakShield();
            Destroy(bullet.gameObject);
        }
        else if (bullet.ultimate)
        {
            Destroy(bullet.gameObject);
        }
        else
        {
            bullet.target = target;
            bullet.tagUse = "Player";

            bullet.gameObject.transform.eulerAngles = new Vector3(0, 0, angle + Random.Range(-10f, 10f));
            bullet.rb.velocity = bullet.gameObject.transform.right * bullet.speed;
            SpriteRenderer spriteRenderer = bullet.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
            bullet.ResetGameObj();
            audioGame.PlayOneShot(shieldDeflec);
        }
    }

    public void ShieldIsOn(bool on)
    {
        if (canGuard)
        {
            shieldCollider.enabled = on;
            shieldSprite.enabled = on;
            conShield = on;
            audioGame.PlayOneShot(on ? shieldOn : shieldOff);
        }
    }

    public void BreakShield()
    {
        canGuard = false;
        var state = gameObject.GetComponentInParent<FSMSEnemySM>();
        if (state)
        {
            redy = false;
            state.stun = true;
            state.ai.monVelocity = Vector2.zero;
            state.animator.ChangeAnimationAttack("Stun");
            state.ChangState(state.checkDistanceState);
        }
        audioGame.PlayOneShot(shieldBreak);
    }
}
