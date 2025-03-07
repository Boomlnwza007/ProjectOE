using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [Header("Atk")]
    public int dmg = 30;

    [Header("Movement")]
    public ParticleSystem particle;
    public float time = 0.5f;
    public float speed = 15f;
    private float timer = 0;
    public Transform player;
    private bool hit;
    public bool move;

    [Header("Movement")]
    public AudioSource audioSource;
    public AudioClip lightning;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {       
        if (timer < time)
        {
            timer += Time.deltaTime;
            if (move)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit)
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageable.Takedamage(dmg,DamageType.Melee,5);
            }
        }
    }

    public void Spark()
    {
        hit = true;
        if (!particle.isPlaying)
        {
            particle.Play();
            audioSource.PlayOneShot(lightning);
        }
    }

}
