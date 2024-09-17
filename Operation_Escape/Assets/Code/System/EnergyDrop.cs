using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrop : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject targetPlayer;
    public float moveSpeed = 1f;
    public float distanceMove = 2f;
    public bool canDestroy;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        if (canDestroy)
        {
            StartCoroutine(Destroy());
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, targetPlayer.transform.position);
        if (distance < distanceMove)
        {
            gotoPlayer();
        }
    }

    IEnumerator Destroy() 
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    void gotoPlayer()
    {
        if (targetPlayer == null)
        {
            return;
        }

        Vector2 playerPos = targetPlayer.transform.position;
        Vector2 target = (playerPos - rb.position).normalized;
        rb.MovePosition(rb.position + target * moveSpeed * Time.fixedDeltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<IEnergy>().GetEnergy(1);
            Destroy(gameObject);
        }
    }
}
