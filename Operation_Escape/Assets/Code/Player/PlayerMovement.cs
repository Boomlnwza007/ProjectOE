using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{  
    private Camera mainCam;
    private float horizontal;
    private float vertical;
    private float dodgeSpeed;
    private Vector2 dodgeDir;
    private Vector3 mousePos;
    private bool canDodge = true;
    public enum State { Normal, Dodge, }
    public State state;
    public float speed = 10f;
    public float dodgeMaxSpeed = 100f;
    public float coolDownDodge = 1f;
    public bool canCombat;
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
                mousePos.z = 0f;
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");               
                if (Input.GetButtonDown("Jump") && canDodge)
                {
                    canDodge = false;
                    dodgeDir = new Vector2(horizontal, vertical).normalized;
                    if (dodgeDir == Vector2.zero)
                    {
                        dodgeDir = (transform.position - mousePos).normalized;
                    }
                    dodgeSpeed = dodgeMaxSpeed;                    
                    state = State.Dodge;
                    StartCoroutine(gameObject.GetComponent<IDamageable>().Imortal(1));
                }
                //Flip();
                break;
            case State.Dodge:
                canCombat = false;
                float dodgeSpeedDropMultiplier = 5f;
                dodgeSpeed -= dodgeSpeed * dodgeSpeedDropMultiplier * Time.deltaTime;
                float dodgeMinimium = 50f;
                if (dodgeSpeed < dodgeMinimium)
                {
                    canCombat = true;
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

    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(coolDownDodge);
        canDodge = true;
    }

}
