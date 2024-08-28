using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerMovement : MonoBehaviour
{  
    private Camera mainCam;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;
    private float rollSpeed;
    private int rollCharge;
    private int maxRollCharge = 3;
    private float rollTimer;
    private float rollChargeCC = 0.5f;
    private float rollCC = 2f;
    private Vector2 dodgeDir;
    private Vector3 mousePos;
    private bool canDodge = true;
    public enum State { Normal, Dodge, }
    public State state;
    public float speed = 10f;
    public float slowSpeed = 0f;
    public float dodgeMaxSpeed = 100f;
    public float coolDownDodge = 1f;
    public bool canCombat;
    public Rigidbody2D rb;

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
                    Roll();                    
                }
                //Flip();
                break;
            case State.Dodge:
                canCombat = false;
                float dodgeSpeedDropMultiplier = 5f;
                rollSpeed -= rollSpeed * dodgeSpeedDropMultiplier * Time.deltaTime;
                float dodgeMinimium = 50f;
                if (rollSpeed < dodgeMinimium)
                {
                    gameObject.GetComponent<IDamageable>().imortal = false;
                    state = State.Normal;
                }
                break;
        }

        if (rollCharge > 0)
        {
            rollTimer += Time.deltaTime;
            if (rollTimer > rollChargeCC)
            {
                rollCharge = 0;
                rollTimer = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                rb.velocity = new Vector2(horizontal * (speed - slowSpeed), vertical * (speed-slowSpeed));
                break;
            case State.Dodge:
                rb.velocity = dodgeDir * (rollSpeed - slowSpeed);
                break;
        }
    }  

    private void Roll()
    {
        if (rollCharge < maxRollCharge)
        {            
            rollCharge++;
            rollTimer = 0;
            dodgeDir = new Vector2(horizontal, vertical).normalized;
            if (dodgeDir == Vector2.zero)
            {
                dodgeDir = (transform.position - mousePos).normalized;
            }
            rollSpeed = dodgeMaxSpeed;
            state = State.Dodge;
            gameObject.GetComponent<IDamageable>().imortal = true;
            rb.velocity = dodgeDir * rollSpeed;
        }
        else
        {
            RollCooldown().Forget();
        }
    }

    async public UniTask RollCooldown()
    {
        canDodge = false;
        await UniTask.WaitForSeconds(rollCC);
        canDodge = true;
        rollCharge = 0;
    }
}
