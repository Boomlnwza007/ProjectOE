using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private List<BaseGun> GunList;
    private int currentGun;
    private int comboStep = 0;
    private float comboTimer;
    private int ammo;
    private int maxAmmo;
    public float comboMaxTime = 1.5f;
    public float comboCooldown = 1f;
    public int maxComboSteps = 3;
    private bool canFire = true;
    private bool firing = true;
    private bool canMelee = true;
    private bool canReload = true;
    private float timer;
    private IEnergy energy;

    // Start is called before the first frame update
    void Start()
    {
        currentGun = 0;
        maxAmmo = GunList[currentGun].maxAmmo;
        ammo = maxAmmo;
        energy = GetComponent<IEnergy>();
    }

    // Update is called once per frame
    void Update()
    {
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

        if (energy.energy > 0)
        {
            if (Input.GetButtonDown("Reload") && canReload)
            {
                energy.UseEnergy(GunList[currentGun].energyUse);
                canReload = false;
                StartCoroutine(Reload(GunList[currentGun].timeReload));
            }
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

    private IEnumerator Reload(float timeReload)
    {
        yield return new WaitForSeconds(timeReload);
        ammo = maxAmmo;
    }
    public void Addgun(BaseGun gun)
    {
        GunList.Add(gun);
    }
}
