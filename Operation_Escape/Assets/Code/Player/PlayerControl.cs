using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    public float speed = 8f;
    private bool facingRight = true;
    private Camera mainCam;
    private Vector3 mousePos;
    public Transform bulletTranform;
    public bool canfire;
    private float timer;
    public float timeBetweenFiring;
    [SerializeField] public GameObject bullet;
    [SerializeField] private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (!canfire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canfire = true;
                timer = 0;
            }
        }

        if (Input.GetAxisRaw("Fire1")==1&&canfire)
        {
            canfire = false;
            Instantiate(bullet, bulletTranform.position, Quaternion.identity);
        }
        Flip();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

    private void Flip()
    {
        if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
