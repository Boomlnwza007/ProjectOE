using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] public List<BaseGun> gunList = new List<BaseGun>();
    [HideInInspector] public GameObject currentEquipGun;
    [HideInInspector] public int currentGun = -1;
    [SerializeField] private GameObject WeaponGun;
    [SerializeField] private Transform aimPoint;
    public bool canReload = true;
    public bool canFire = true;
    public float UltiTime = 0;
    private Coroutine reloadCoroutine;

    [Header("Melee")]
    public int damage = 1;
    public float comboCooldown = 1f;
    public bool canMelee = true;
    public Vector2 sizeHitbox;
    public GameObject meleeHand;
    public GameObject meleeZone;
    public bool isAttackRight;

    [Header("Status")]
    public float knockBack = 1;
    private IEnergy energy;
    private bool fireReload = true;

    // Start is called before the first frame update
    void Start()
    {
        currentGun = 0;
        energy = GetComponent<IEnergy>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        if (gunList.Count <= 0)
        {
            return;
        }

        if (!gunList[currentGun].firing)
        {
            gunList[currentGun].fireRate += Time.deltaTime;
            if (gunList[currentGun].fireRate > gunList[currentGun].maxFireRate)
            {
                gunList[currentGun].firing = true;
                gunList[currentGun].fireRate = 0;
            }
        }

        if (gunList[currentGun].canSpecial)
        {
            gunList[currentGun].Special();
        }       

        if (gunList[currentGun].canUltimate)
        {
            TimeUltimate();
        }
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("Fire2") && canMelee)
        {
            ComboAttack();
        }

        if (gunList.Count <= 0)
        {
            return;
        }    

        if (Input.GetButton("Fire1") && canFire && gunList[currentGun].firing)
        {
            HandleFire();
        }

        if (Input.GetButtonUp("Fire1") && canFire && gunList[currentGun].firing)
        {
            fireReload = true;
        }

        if (Input.GetButtonDown("Reload") && canReload && !gunList[currentGun].canUltimate)
        {
            Reload();
        }

        if (Input.GetButton("Ultimate") && !gunList[currentGun].canUltimate && energy.ultimateEnergy == 10 && canReload)
        {
            energy.canGetUltimateEnergy = false;
            gunList[currentGun].Ultimate();
            Debug.Log("Ultimate");
        }

        HandleWeaponSwitch();
    }

    private void HandleFire()
    {
        if (gunList[currentGun].ammo > 0)
        {
            gunList[currentGun].Fire();
        }
        else
        {
            if (canReload & fireReload)
            {
                Reload();
            }
        }
    }

    private void ComboAttack()
    {
        canMelee = false;
        canFire = false;
        PlayerControl.control.Slow(100);
        if (gunList.Count > 0)
        {
            gunList[currentGun].Remove();
        }
        isAttackRight = PlayerControl.control.animator.GetBool("Right");
        PlayerControl.control.animator.SetTrigger("Attack");
        StartCoroutine(ComboDelay(comboCooldown));
    }

    public void PreAttack()
    {
        if (isAttackRight)
        {
            Vector3 newVector = new Vector3(1,1,1);
            meleeHand.transform.localScale = newVector;
        }
        else
        {
            Vector3 newVector = new Vector3(-1, 1, 1);
            meleeHand.transform.localScale = newVector;
        }
        meleeZone.SetActive(true);
        Invoke("Attack", 0.1f);
    }

    public void Attack()
    {       
        Collider2D _colliders = meleeZone.GetComponent<Collider2D>();
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Physics2D.OverlapCollider(_colliders, filter, colliders);

        foreach (var hit in colliders)
        {
            if (hit.gameObject != gameObject && hit.TryGetComponent(out IDamageable dam))
            {
                dam.Takedamage(damage, DamageType.Melee, knockBack);
                foreach (Transform child in hit.transform)
                {
                    BulletSticky bullet = child.GetComponent<BulletSticky>();
                    if (bullet != null)
                    {
                        bullet.DelAFMelee();
                    }
                }
                Debug.Log(hit.name);
            }
            else if (hit.TryGetComponent(out IBulletInteract bulletInteract))
            {
                bulletInteract.Interact(DamageType.Melee);
            }
        }
        meleeZone.SetActive(false);
        PlayerControl.control.Slow(0);
    }

    private void TimeUltimate()
    {
        if (UltiTime >= gunList[currentGun].timeUltimate)
        {
            ReUltimate();
        }
        else
        {
            UltiTime += Time.deltaTime;
            energy.ultimateEnergy = (int)Mathf.Lerp(10f, 0f, UltiTime / gunList[currentGun].timeUltimate);
        }
    }

    public void ReUltimate()
    {
        gunList[currentGun].canUltimate = false;
        gunList[currentGun].RemoveUltimate();
        energy.canGetUltimateEnergy = true;
        energy.ultimateEnergy = 0;
        UltiTime = 0;
        gunList[currentGun].ammo = gunList[currentGun].maxAmmo;
    }

    public void Reload()
    {
        if (energy.energy >= gunList[currentGun].energyUse && gunList[currentGun].ammo != gunList[currentGun].maxAmmo)
        {
            Debug.Log("Reload");
            canReload = false;
            canFire = false;
            AudioManager.audioManager.PlaySFX(PlayerSound.playerSound.reload);
            reloadCoroutine = StartCoroutine(ReloadWait(gunList[currentGun].timeReload));
        }
        else
        {
            fireReload = false;
            AudioManager.audioManager.PlaySFX(PlayerSound.playerSound.gunFail);
        }
    }

    private IEnumerator ReloadWait(float timeReload)
    {
        yield return new WaitForSeconds(timeReload);
        energy.UseEnergy(gunList[currentGun].energyUse);
        canReload = true;
        canFire = true;
        gunList[currentGun].ammo = gunList[currentGun].maxAmmo;
        
    }

    private IEnumerator ComboDelay(float wait)
    {
        yield return new WaitForSeconds(wait);
        canMelee = true;
        canFire = true;        
    }

    public void Addgun(BaseGun gun)
    {
        if (gunList.Count != 0)
        {
            gunList[currentGun].Remove();
            gunList[currentGun].Exit();
            if (gunList[currentGun].canUltimate)
            {
                gunList[currentGun].canUltimate = false;
                energy.canGetUltimateEnergy = true;
                energy.ultimateEnergy = 0;
                UltiTime = 0;
            }
        }       

        currentGun = gunList.Count;
        gunList.Add(gun);
        gunList[currentGun].Setup();
        aimPoint.localPosition = new Vector3(gunList[currentGun].aimDistance, 0, 0);
        gunList[currentGun].bulletTranform = aimPoint;
        PlayerControl.control.ammoBar.gameObject.SetActive(true);
        EquipGun(currentGun);
    }

    public void RemoveGun(BaseGun gun)
    {
        BaseGun oldGun = gunList[currentGun];

        for (int i = 0; i < gunList.Count; i++)
        {
            if (gunList[i] == gun)
            {
                gunList[i].Remove();
                gunList[i].Exit();
                gunList.RemoveAt(i);
            }
        }

        for (int i = 0; i < gunList.Count; i++)
        {
            if (gunList[i] == oldGun)
            {
                currentGun = i;
            }
        }

        if (gun == oldGun)
        {
            if (gunList.Count != 0)
            {
                int index = (currentGun - 1 + gunList.Count) % gunList.Count;
                currentGun = index;
                gunList[currentGun].bulletTranform = aimPoint;
                if (reloadCoroutine != null)
                {
                    StopCoroutine(reloadCoroutine);
                    reloadCoroutine = null;
                }
                gunList[currentGun].firing = false;
                EquipGun(currentGun);
            }
        }

        if (gunList.Count < 1)
        {
            Destroy(currentEquipGun);
            PlayerControl.control.ammoBar.gameObject.SetActive(false);
        }
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
            index = scroll > 0 ? (currentGun + 1) % gunList.Count : (currentGun - 1 + gunList.Count) % gunList.Count;
            SwapGun(index);
        }
    }

    public void SwapGun(int index)
    {
        if (index + 1 > gunList.Count || index == currentGun)
        {
            return;
        }

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        gunList[currentGun].Remove();
        gunList[currentGun].Exit();
        if (gunList[currentGun].canUltimate)
        {
            gunList[currentGun].canUltimate = false;
            energy.canGetUltimateEnergy = true;
            energy.ultimateEnergy = 0;
            UltiTime = 0;
        }

        currentGun = index;
        aimPoint.localPosition = new Vector3(gunList[currentGun].aimDistance, 0, 0);
        gunList[currentGun].bulletTranform = aimPoint;
        canFire = true;
        canReload = true;
        EquipGun(index);
    }

    public void EquipGun(int index)
    {
        if (currentEquipGun != null)
        {
            Destroy(currentEquipGun);
        }

        currentEquipGun = Instantiate(gunList[index].gunPrefab, WeaponGun.transform);
        PlayerControl.control.guntSprite = currentEquipGun.GetComponentInChildren<SpriteRenderer>();
        gunList[index].Enter();
        PlayerControl.control.ammoBar.value = gunList[index].energyUse;
        AudioManager.audioManager.PlaySFX(PlayerSound.playerSound.ChangGun);
    }

    private void OnDrawGizmos()
    {

        if (Application.isPlaying)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3 aimDir = (mousePos - aimPoint.position).normalized;

            Vector2 castDirection = aimDir.x > 0 ? Vector2.right : Vector2.left;

            Vector2 boxCastSize = new Vector2(sizeHitbox.x, sizeHitbox.y);
            Vector2 boxCastOrigin = new Vector2(aimPoint.position.x, aimPoint.position.y);

            Gizmos.color = Color.red;
            Vector2 gizmoSize = new Vector2(boxCastSize.x, boxCastSize.y);
            Gizmos.DrawWireCube(boxCastOrigin + (castDirection * gizmoSize.x / 2), gizmoSize);
        }
    }
}
