using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour ,IDamageable
{
    [SerializeField] private List<BaseGun> GunList;
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
    private int comboStep = 0;
    private float comboTimer;
    private int currentGun;
    private int ammo;
    private int maxAmmo;
    public int Health = 10;
    public Transform bulletTranform;
    public float speed = 10f;
    public float dodgeMaxSpeed=100f;
    public float coolDownDodge = 1f;
    public float timeBetweenFiring;
    public float comboMaxTime = 1.5f;
    public float comboCooldown = 1f;
    public int maxComboSteps = 3;
    private bool canFire = true;
    private bool firing = true;
    private bool canMelee = true;
    [SerializeField] public GameObject bullet;
    [SerializeField] private Rigidbody2D rb;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        state = State.Normal;
        currentGun = 0;
        maxAmmo = GunList[currentGun].maxAmmo;
        ammo = maxAmmo;
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

                if (!firing)
                {
                    timer += Time.deltaTime;
                    if (timer > GunList[currentGun].fireRate)
                    {
                        firing = true;
                        timer = 0;
                    }
                }

                if (ammo > 0)
                {
                    if (Input.GetButton("Fire1") && canFire && firing)
                    {
                        ammo--;
                        firing = false;
                        comboStep = 0;
                        comboTimer = 0;
                        GunList[currentGun].Fire();
                        //Instantiate(bullet, bulletTranform.position, Quaternion.identity);
                    }
                }
                

                if (Input.GetButtonDown("Fire2") && canMelee)
                {
                    ComboAttack();
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

        if (comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboMaxTime)
            {
                comboStep = 0;
                comboTimer = 0;
            }
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

    private void ComboAttack()
    {
        if (comboStep < maxComboSteps)
        {
            comboStep++;
            comboTimer = 0;
            canMelee = false;
            canFire = false;
            Debug.Log("Combo Attack Step: " + comboStep);

         
            Instantiate(bullet, bulletTranform.position, Quaternion.identity);

            StartCoroutine(ComboDelay());
        }
        else
        {
            canMelee = false;
            StartCoroutine(ComboCooldown());
        }
    }

    private IEnumerator ComboDelay()
    {
        yield return new WaitForSeconds(1f);
        canMelee = true;
        canFire = true;
    }

    private IEnumerator ComboCooldown()
    {
        yield return new WaitForSeconds(comboCooldown);
        comboStep = 0;
        comboTimer = 0;
        canMelee = true;
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

    IEnumerator Imortal(float wait) 
    {
        yield return new WaitForSeconds(wait);
    }

    public void Addgun(BaseGun gun)
    {
        GunList.Add(gun);
    }

    public void Takedamage(int damage, DamageType type)
    {
        Health -= damage;
        if (Health<=0)
        {
            Die();
        }
    }

    public void Die()
    {

    }


}
