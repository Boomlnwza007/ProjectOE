using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerMovement : MonoBehaviour
{
    public enum State { Normal, Dodge, }

    [Header("Status")]
    public State state;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;
    public bool canCombat;
    public Rigidbody2D rb;
    private IDamageable damageable;

    [Header("Walk")]
    public float speed = 10f;
    public float slowSpeed = 0f;


    [Header("Roll")]
    public float dodgeMaxSpeed = 100f;
    public float dodgeMinimium = 50f;
    public float dodgeSpeedDropMultiplier = 5f;
    public float coolDownDodge = 1f;
    public float rollChargeCC = 1f;
    private float rollCC = 2f;
    private Vector2 dodgeDir;
    private Vector3 mousePos;
    private bool canDodge = true;
    private float rollSpeed;
    private int maxRollCharge = 3;
    private float rollTimer;
    [HideInInspector] public int rollCharge;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = State.Normal;
        damageable = GetComponent<IDamageable>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");      
                
                if (Input.GetButtonDown("Jump") && canDodge)
                {
                    Roll();                    
                }

                break;
            case State.Dodge:
                canCombat = false;
                rollSpeed -= rollSpeed * dodgeSpeedDropMultiplier * Time.deltaTime;
                if (rollSpeed < dodgeMinimium)
                {
                    damageable.imortal = false;
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
                rb.velocity = new Vector2(horizontal , vertical).normalized * (speed - slowSpeed);
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
            PlayerControl.control.animator.SetBool("RollRightDir", horizontal == 1);
            if (dodgeDir == Vector2.zero)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                dodgeDir = (transform.position - mousePos).normalized;
                PlayerControl.control.animator.SetBool("RollRightDir", !PlayerControl.control.animator.GetBool("Right"));
            }

            rollSpeed = dodgeMaxSpeed;
            state = State.Dodge;
            damageable.imortal = true;
            PlayerControl.control.animator.SetTrigger("Roll");
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
