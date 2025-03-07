using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("Player")]
    public static PlayerControl control;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    public PlayerState playerState;
    public List<int> key = new List<int>();
    [SerializeField] private PlayerAim playerAim;
    public Animator animator;
    public bool isdaed;
    public GameObject feet;
    private bool showgun = true;
    [HideInInspector] public bool isFacingRight;

    [Header("UI")]
    [SerializeField] private GameObject UI;
    private bool UIOnUpdate = true;
    [SerializeField] private SliderBar healthBar;
    [SerializeField] private SliderBar ultimateEnergyBar;
    [SerializeField] private SliderBar ultimateEnergyBarCircle;
    [SerializeField] private SliderBar energyBar;
    [SerializeField] private SliderBar reloadBar;
    [SerializeField] private OBJBar bulletBar;
    [SerializeField] private PauseScene menu;
    [SerializeField] public Slider ammoBar;
    [SerializeField] public GameObject healUseBar;

    private float reloadTime = 0;
    private string currentGunName;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerState = GetComponent<PlayerState>();
        control = this;
    }

    private void Start()
    {
        healthBar.SetMax(playerState.maxHealth, playerState.health);
        energyBar.SetMax(playerState.maxEnergy, playerState.energy);
        ultimateEnergyBar.SetMax(playerState.maxUltimateEnergy, playerState.ultimateEnergy);
        ultimateEnergyBarCircle.SetMax(playerState.maxUltimateEnergy, playerState.ultimateEnergy);
        reloadBar.SetMax(100, 0);
    }

    private void Update()
    {        

        if (UIOnUpdate)
        {
            if (Input.GetButtonDown("Pause"))
            {
                menu.HidePause();
            }

            UpdateBars();
            UpdateBullets();
            HandleReload();
        }


        if (menu.onPauseMenu || menu.onmenuAfterDie)
        {
            return;
        }

        UpdateAnimation();

    }

    private void UpdateAnimation()
    {
        isFacingRight = playerAim.angle > -90 && playerAim.angle < 90;
        bool HasGun = playerCombat.gunList.Count > 0 && showgun;

        animator.SetBool("Right", isFacingRight);

        animator.SetBool("HasGun", HasGun);

        if (playerMovement.horizontal != 0 || playerMovement.vertical != 0)
        {
            bool isMovingBackwards = (isFacingRight && playerMovement.horizontal < 0) || (!isFacingRight && playerMovement.horizontal > 0);
            animator.SetBool("MoveBackwards", isMovingBackwards);
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }

    private void HandleReload()
    {
        if (reloadBar == null || playerCombat.gunList.Count == 0) return;

        var currentGun = playerCombat.gunList[playerCombat.currentGun];

        if (!playerCombat.canReload)
        {
            if (currentGunName != currentGun.name)
            {
                reloadTime = 0;
                reloadBar.value = 0;
                currentGunName = currentGun.name;
            }

            if (reloadTime < currentGun.timeReload)
            {
                reloadBar.Off(true);
                reloadTime += Time.deltaTime;
                reloadBar.value = Mathf.Lerp(0, 100f, reloadTime / currentGun.timeReload);
            }
        }
        else if (reloadBar.canShow)
        {
            reloadBar.Off(false);
            reloadTime = 0;
            reloadBar.value = 0;
        }
    }

    private void UpdateBars()
    {
        if (healthBar != null && playerState.health != healthBar.value)
        {
            healthBar.SetValue(playerState.health);
        }

        if (energyBar != null && playerState.energy != energyBar.value)
        {
            energyBar.SetValue(playerState.energy);
        }

        if (ultimateEnergyBar != null && playerState.ultimateEnergy != ultimateEnergyBar.value)
        {
            ultimateEnergyBar.SetValue(playerState.ultimateEnergy);
        }

        if (ultimateEnergyBarCircle != null && playerState.ultimateEnergy != ultimateEnergyBarCircle.value)
        {
            ultimateEnergyBarCircle.SetValue(playerState.ultimateEnergy);
        }
    }

    private void UpdateBullets()
    {
        if (bulletBar == null || playerCombat.gunList.Count == 0)
        {
            if (bulletBar != null && bulletBar.bar.activeSelf)
            {
                bulletBar.bar.SetActive(false);
            }
            return;
        }

        if (!bulletBar.bar.activeSelf)
        {
            bulletBar.bar.SetActive(true);
        }

        var currentGun = playerCombat.gunList[playerCombat.currentGun];

        if (bulletBar.ultimate != currentGun.canUltimate)
        {
            bulletBar.ultimate = currentGun.canUltimate;
            bulletBar.ChangPrefab(bulletBar.ultimate);
           
        }

        if (currentGun.ammo != bulletBar.value)
        {
            bulletBar.SetValue(currentGun.ammo);
        }

        if (bulletBar.obj.nameItem[bulletBar.curgun] != currentGun.name)
        {
            bulletBar.SetUp(currentGun.name,false);
        }        
    }

    public void EnableInput(bool enable)
    {
        if (!enable)
        {
            playerMovement.rb.velocity = Vector2.zero;
            playerMovement.state = PlayerMovement.State.Normal;
        }
        playerMovement.enabled = enable;
        playerCombat.enabled = enable;
        playerAim.enabled = enable;
    }

    public void EnableUI(bool enable)
    {
        UIOnUpdate = enable;
        playerState.canHealth = enable;
        UI.SetActive(enable);
    }


    public void Slow(float percent)
    {
        playerMovement.slowSpeed = playerMovement.speed * (percent / 100f);
    }

    public void ResetSlow()
    {
        playerMovement.slowSpeed = 0;
    }

    public void Spawn(Transform spawnPoint)
    {
        if (spawnPoint != null)
        {
            gameObject.transform.position = spawnPoint.position;
            playerState.health = playerState.maxHealth;
            playerState.energy = playerState.maxEnergy;
            playerState.ultimateEnergy = 0;
            gameObject.SetActive(true);
        }    
    }

    public void ShowGameOver()
    {
        menu.HideAfterDie();
    }

    public void ShowGun(bool on)
    {
        if (playerCombat.currentEquipGun!=null)
        {
            showgun = on;
            playerCombat.weaponGun.SetActive(on);
        }
    }
}
