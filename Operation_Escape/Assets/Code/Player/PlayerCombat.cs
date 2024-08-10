using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private bool firing = true;
    private IEnergy energy;
    private GameObject currentEquipGun;
    [SerializeField] private GameObject WeaponGun;
    [SerializeField] private Transform aimPoint;
    private int currentGun = -1;
    private int comboStep = 0;
    private float comboTimer;
    [SerializeField] public List<BaseGun> gunList = new List<BaseGun>();
    public Vector2 sizeHitbox;
    public int damage = 1;
    public float knockBack = 1;
    public float comboMaxTime = 1.5f;
    public float comboCooldown = 1f;
    public int maxComboSteps = 3;
    public bool canMelee = true;
    public bool canReload = true;
    public bool canFire = true;


    // Start is called before the first frame update
    void Start()
    {
        aimPoint = GameObject.FindGameObjectWithTag("Aim").GetComponent<Transform>();
        currentGun = 0;
        energy = GetComponent<IEnergy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && canMelee)
        {
            ComboAttack();
        }

        if (gunList.Count <= 0)
        {
            return;
        }

        HandleWeaponSwitch();

        if (!firing)
        {
            gunList[currentGun].fireRate += Time.deltaTime;
            if (gunList[currentGun].fireRate > gunList[currentGun].maxFireRate)
            {
                firing = true;
                gunList[currentGun].fireRate = 0;
            }
        }

        if (Input.GetButton("Fire1") && canFire && firing)
        {
            if (gunList[currentGun].ammo > 0)
            {
                firing = false;
                comboStep = 0;
                comboTimer = 0;
                CinemachineControl.Instance.ShakeCamera(1f , 0.2f);
                gunList[currentGun].Fire();
                //Instantiate(bullet, bulletTranform.position, Quaternion.identity);
            }
            else
            {
                if (canReload)
                {
                    Debug.Log("Reload");
                    Reload();
                }
            }
        }     
                

        if (Input.GetButtonDown("Reload") && canReload)
        {
            Debug.Log("Reload");
            Reload();
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

    private void ComboAttack()
    {
        if (comboStep < maxComboSteps)
        {
            Debug.Log("Melee");
            comboStep++;
            comboTimer = 0;
            canMelee = false;
            canFire = false;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3 aimDir = (mousePos - aimPoint.position).normalized;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            RaycastHit2D[] hitEnemies = null;
            Debug.Log("Combo Attack Step: " + comboStep);
            switch (comboStep)
            {
                case (1) :
                    sizeHitbox = new Vector2(3, 2);
                    hitEnemies = Physics2D.BoxCastAll(aimPoint.position, sizeHitbox, angle, Vector2.right);
                    break;
                case (2):
                    sizeHitbox = new Vector2(2, 3);
                    hitEnemies = Physics2D.BoxCastAll(aimPoint.position, sizeHitbox, angle, Vector2.right);
                    break;
                case (3):
                    sizeHitbox = new Vector2(5, 1);
                    hitEnemies = Physics2D.BoxCastAll(aimPoint.position, sizeHitbox, angle, Vector2.right);
                    break;
            }

            foreach (RaycastHit2D hit in hitEnemies)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.GetComponent<IDamageable>() != null)
                    {
                        hit.collider.GetComponent<IDamageable>().Takedamage(damage*comboStep, DamageType.Melee,knockBack);
                        Debug.Log(hit.collider.name);
                    }
                }
            }
            StartCoroutine(ComboDelay(1f));
        }
        else
        {
            canMelee = false;
            StartCoroutine(ComboCooldown());
        }
    }

    public void Reload()
    {
        if (energy.energy > 0)
        {
            canReload = false;
            StartCoroutine(Reload(gunList[currentGun].timeReload));
        }
    }

    private IEnumerator ComboDelay(float wait)
    {
        yield return new WaitForSeconds(wait);
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

    private IEnumerator Reload(float timeReload)
    {
        yield return new WaitForSeconds(timeReload);
        energy.UseEnergy(gunList[currentGun].energyUse);
        canReload = true;
        gunList[currentGun].ammo = gunList[currentGun].maxAmmo;
    }
    public void Addgun(BaseGun gun)
    {
        currentGun = gunList.Count;
        gunList.Add(gun);
        gunList[currentGun].bulletTranform = aimPoint;
        EquipGun(currentGun);
    }

    private void HandleWeaponSwitch()
    {
        for (int i = 0; i < gunList.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SwapGun(i);
                return;
            }
        }

        int index;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                index = (currentGun + 1) % gunList.Count;
            }
            else
            {
                index = (currentGun - 1 + gunList.Count) % gunList.Count;
            }
            SwapGun(index);
        }
    }


    public void SwapGun(int index)
    {
        Debug.Log(index + 1 +" > "+ currentGun+" && "+ index +" == " +currentGun);
        if (index+1 > gunList.Count || index == currentGun)
        {            
            return;
        }

        currentGun = index;
        gunList[currentGun].bulletTranform = aimPoint;
        StopCoroutine(Reload(gunList[currentGun].timeReload));
        //canFire = true;
        firing = false;
        //canMelee = false;
        //canReload = false;
        EquipGun(index);
    }

    private void EquipGun(int index)
    {
        if (currentEquipGun != null)
        {
            Destroy(currentEquipGun);
        }

        currentEquipGun = Instantiate(gunList[index].gunPrefab, WeaponGun.transform);
    }

    private void OnDrawGizmos()
    {
        if (aimPoint == null)
            return;

        // แสดงขนาดของ Hitbox ด้วย Gizmo
        Gizmos.color = Color.red;
        Vector3 center = aimPoint.position;
        Gizmos.DrawWireCube(center, sizeHitbox);

        // วาดเส้นเพื่อแสดงทิศทางของ BoxCast
        Gizmos.color = Color.blue;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 aimDir = (mousePos - aimPoint.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
        Gizmos.DrawLine(center, center + direction * sizeHitbox.x / 2);
    }
}
