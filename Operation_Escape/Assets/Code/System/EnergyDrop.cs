using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnergyDrop : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject targetPlayer;
    public float moveSpeed = 1f;
    public float distanceMove = 2f;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        Destroy().Forget();
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

    async UniTask Destroy()
    {
        await UniTask.WaitForSeconds(10f);
        Destroy(gameObject);
    }

    void gotoPlayer()
    {
        if (GameObject.Find("player") == null)
        {
            return;
        }

        Vector2 playerPos = targetPlayer.transform.position;
        Vector2 target = (playerPos - rb.position).normalized;
        rb.MovePosition(rb.position + target * moveSpeed * Time.fixedDeltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<IEnergy>().GetEnergy(1);
            Destroy(gameObject);

        }
    }
}