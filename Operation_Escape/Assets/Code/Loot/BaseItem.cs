using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected GameObject targetPlayer;
    public float moveSpeed = 1f;
    public float distanceMove = 2f;
    public float destroyTime = 10f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPlayer = GameObject.Find("player");
        StartCoroutine(DestroyAfterTime(destroyTime));
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (targetPlayer == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, targetPlayer.transform.position);
        if (distance < distanceMove)
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    protected void MoveTowardsPlayer()
    {
        if (targetPlayer == null)
        {
            return;
        }

        Vector2 playerPos = targetPlayer.transform.position;
        Vector2 direction = (playerPos - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            Abilities(collision);
        }
    }

    protected abstract void Abilities(Collider2D collision);
}
