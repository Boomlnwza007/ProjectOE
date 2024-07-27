using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private List<BaseGun> GunList;
    private enum State { Normal, Dodge, }
    private Camera mainCam;
    private State state;
    private float horizontal;
    private float vertical;
    private float dodgeSpeed;
    private bool facingRight = true;
    private Vector3 dodgeDir;
    private Vector3 mousePos;
    private bool canDodge = true;
    public float speed = 10f;
    public float dodgeMaxSpeed = 100f;
    public float coolDownDodge = 1f;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        state = State.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");
               
                if (Input.GetButtonDown("Jump") && canDodge)
                {
                    dodgeDir = new Vector3(horizontal, vertical).normalized;
                    StartCoroutine(gameObject.GetComponent<IDamageable>().Imortal(1));          
                    if (dodgeDir == Vector3.zero)
                    {
                        dodgeDir = (transform.position - mousePos).normalized;
                    }
                    StartCoroutine(DodgeCooldown());
                    dodgeSpeed = dodgeMaxSpeed;
                    canDodge = false;
                    state = State.Dodge;
                }
                //Flip();
                break;
            case State.Dodge:
                float dodgeSpeedDropMultiplier = 5f;
                dodgeSpeed -= dodgeSpeed * dodgeSpeedDropMultiplier * Time.deltaTime;

                float dodgeMinimium = 50f;
                if (dodgeSpeed < dodgeMinimium)
                {
                    state = State.Normal;
                    StartCoroutine(DodgeCooldown());
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                rb.velocity = new Vector2(horizontal * speed, vertical * speed);
                break;
            case State.Dodge:
                rb.velocity = dodgeDir * dodgeSpeed;
                break;
        }
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

    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(coolDownDodge);
        canDodge = true;
    }

}
