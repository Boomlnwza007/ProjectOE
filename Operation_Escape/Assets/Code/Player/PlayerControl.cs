using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private enum State { Normal,Dodge,}
    private State state;
    private float horizontal;
    private float vertical;
    private float dodgeSpeed;
    private float timer;
    private bool facingRight = true;
    private Vector3 dodgeDir;
    private Vector3 mousePos;
    private Camera mainCam;
    private bool canDodge = true;
    public Transform bulletTranform;
    public float speed = 8f;
    public float dodgeMaxSpeed=100f;
    public float coolDownDodge = 1f;
    public float timeBetweenFiring;
    public bool canfire;
    [SerializeField] public GameObject bullet;
    [SerializeField] private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        state = State.Normal;
    }
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
                if (!canfire)
                {
                    timer += Time.deltaTime;
                    if (timer > timeBetweenFiring)
                    {
                        canfire = true;
                        timer = 0;
                    }
                }

                if (Input.GetButton("Fire1") && canfire)
                {
                    canfire = false;
                    Instantiate(bullet, bulletTranform.position, Quaternion.identity);
                }

                if (Input.GetButtonDown("Jump") && canDodge) 
                {
                    dodgeDir = new Vector3(horizontal, vertical).normalized;
                    if (dodgeDir == Vector3.zero)
                    {
                        dodgeDir = (transform.position - mousePos).normalized;
                    }
                    dodgeSpeed = dodgeMaxSpeed;
                    canDodge = false;
                    state = State.Dodge;
                }
                Flip();
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
                rb.velocity = dodgeDir*dodgeSpeed;
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
