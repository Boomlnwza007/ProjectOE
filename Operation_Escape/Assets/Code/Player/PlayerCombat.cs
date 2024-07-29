using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] public List<BaseGun> gunList = new List<BaseGun>();
    private int currentGun = -1;
    private int comboStep = 0;
    public int ammo;
    private int maxAmmo;
    private float comboTimer;
    public float comboMaxTime = 1.5f;
    public float comboCooldown = 1f;
    public int maxComboSteps = 3;
    private bool canFire = true;
    private bool firing = true;
    private bool canMelee = true;
    private bool canReload = true;
    private bool canSwap = true;
    private float timer;
    private IEnergy energy;
    private GameObject currentEquipGun;
    [SerializeField]private GameObject WeaponGun;
    private Transform AimPoint;

    // Start is called before the first frame update
    void Start()
    {
        AimPoint = GameObject.FindGameObjectWithTag("Aim").GetComponent<Transform>();
        currentGun = 0;
        if (gunList.Count > 0)
        {
            maxAmmo = gunList[currentGun].maxAmmo;
            ammo = maxAmmo;
        }
        energy = GetComponent<IEnergy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gunList.Count <= 0)
        {
            return;
        }

        if (canSwap)
        {
            HandleWeaponSwitch();
        }

        if (!firing)
        {
            timer += Time.deltaTime;
            if (timer > gunList[currentGun].fireRate)
            {
                firing = true;
                canSwap = true;
                timer = 0;
            }
        }

        if (Input.GetButton("Fire1") && canFire && firing)
        {
            if (ammo > 0)
            {
                ammo--;
                firing = false;
                canSwap = false;
                comboStep = 0;
                comboTimer = 0;
                gunList[currentGun].ammo = ammo;
                gunList[currentGun].Fire();
                //Instantiate(bullet, bulletTranform.position, Quaternion.identity);
            }
        }        
        else
        {
            Reload();
        }

        if (Input.GetButtonDown("Fire2") && canMelee)
        {
            ComboAttack();
        }

        if (Input.GetButtonDown("Reload") && canReload)
        {
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
            comboStep++;
            comboTimer = 0;
            canMelee = false;
            canFire = false;
            Debug.Log("Combo Attack Step: " + comboStep);

            //Instantiate(bullet, bulletTranform.position, Quaternion.identity);
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
            energy.UseEnergy(gunList[currentGun].energyUse);
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
        ammo = maxAmmo;
    }
    public void Addgun(BaseGun gun)
    {
        currentGun = gunList.Count;
        gunList.Add(gun);
        gunList[currentGun].bulletTranform = AimPoint;
        maxAmmo = gunList[currentGun].maxAmmo;
        ammo = gunList[currentGun].ammo;
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
        gunList[currentGun].bulletTranform = AimPoint;
        maxAmmo = gunList[currentGun].maxAmmo;
        ammo = gunList[currentGun].ammo;
        //canFire = true;
        //firing = false;
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
}
