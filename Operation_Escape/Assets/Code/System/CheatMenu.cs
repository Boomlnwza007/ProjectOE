using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatMenu : MonoBehaviour
{
    [Header("SpawnMon")]
    [SerializeField] private GameObject buttonPrefabSpawn;
    [SerializeField] private ID listenemy;
    public Transform menuSpawn;
    private List<GameObject> enemySpawnNow = new List<GameObject>();
    private int curPageSpawn = 0;

    [Header("Player")]
    private bool[] onPlayer = new bool[6];
    private Dictionary<BaseBullet, int> originalDamages = new Dictionary<BaseBullet, int>();
    private List<BaseBullet> bullets = new List<BaseBullet>();

    [Header("GunEditer")]
    [SerializeField] private GameObject buttonPrefabGun;
    [SerializeField] private ID listGun;
    private List<GameObject> addButtun = new List<GameObject>();
    public Transform menuGun;
    private int curPageGun = 0;

    [Header("Other")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject mainMenu;
    public bool onMainMenu = false;
    private GameObject player;
    public enum Mode { Spawnmon ,PlayerStatus ,GunEdit };
    [SerializeField] private GameObject[] allMenu;
    public Mode mode;
    private int curPage = 0;
    private int maxPage;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mode = Mode.Spawnmon;
        mainMenu.SetActive(onMainMenu);
    }

    void Start()
    {
        canvas.worldCamera = Camera.main;
        ChangMode(Mode.Spawnmon);
        ModeGun();
        ModeSpawn();
    }

    private void Update()
    {

        CheckListGun();
        CheckPlaystatus();
        CheckListMon();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
        {
            onMainMenu = !onMainMenu;
            mainMenu.SetActive(onMainMenu);
        }
    }

    public void ModeSpawn()
    {
        if (listenemy != null)
        {
            for (int i = 0; i < listenemy.Item.Length; i++)
            {
                GameObject button = Instantiate(buttonPrefabSpawn, menuSpawn);
                button.name = i.ToString();
                button.GetComponentInChildren<TMP_Text>().text = listenemy.Item[i].name;
            }
        }
        curPage = curPageSpawn;
        maxPage = listenemy.Item.Length / 5;
    }

    public void OnClickSpawn(Button button)
    {
        string buttonName = button.name;
        int buttonNumber;

        // Try to parse the button's name into an integer
        if (int.TryParse(buttonName, out buttonNumber))
        {
            SpawnMon(buttonNumber);
        }
        else
        {
            Debug.LogError("Button name is not a valid integer: " + buttonName);
        }
    }

    public void SpawnMon(int enemy)
    {
        Vector3 pos = CinemachineControl.Instance.player.position;
        pos += (Random.insideUnitSphere * 15);
        pos.z = 0;
        Debug.Log(enemy);
        GameObject _enemy = Instantiate(listenemy.Item[enemy], pos, Quaternion.identity);
        enemySpawnNow.Add(_enemy);
    }

    public void DestroyMonstersOnScreen()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject monster in monsters)
        {
            if (IsVisible(monster))
            {
                Destroy(monster);
            }
        }
    }

    bool IsVisible(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer.isVisible;
        }

        return false;
    }

    public void CheckListMon()
    {
        for (int i = 0; i < enemySpawnNow.Count; i++)
        {
            if (enemySpawnNow[i] == null)
            {
                enemySpawnNow.RemoveAt(i);
            }
        }
    }

    public void ModeGun()
    {
        if (listGun != null)
        {
            for (int i = 0; i < listGun.Item.Length; i++)
            {
                GameObject button = Instantiate(buttonPrefabGun, menuGun);
                addButtun.Add(button);
                button.name = listGun.Item[i].name;
                button.GetComponentInChildren<Button>().gameObject.name = i.ToString();
                button.GetComponentInChildren<TMP_Text>().text = listGun.Item[i].name;
                TMP_Text text = button.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<TMP_Text>();
                foreach (var gun in player.GetComponent<PlayerCombat>().gunList)
                {                    
                    if (listGun.Item[i].name == gun.name)
                    {
                        text.text = "Remove";
                        break;
                    }
                    else
                    {
                        text.text = "Add";
                    }
                }
            }
        }
        curPage = curPageGun;
        maxPage = listGun.Item.Length / 5;
    }

    public void OnClickGun(Button button)
    {
        string buttonName = button.name;
        int buttonNumber;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        PlayerCombat playerCombat = player.GetComponent<PlayerCombat>(); 
        BaseGun gun = null;

        if (int.TryParse(buttonName, out buttonNumber))
        {
            gun = listGun.Item[buttonNumber].GetComponent<BaseGun>();
        }
        else
        {
            Debug.LogError("Button name is not a valid integer: " + buttonName);
        }

        TMP_Text text = button.GetComponentInChildren<TMP_Text>();

        switch (text.text)
        {
            case "Add":
                if (gun != null)
                {
                    playerCombat.Addgun(gun);
                }
                button.GetComponentInChildren<TMP_Text>().text = "Remove";
                break;
            case "Remove":
                if (gun != null)
                {
                    playerCombat.RemoveGun(gun);
                }
                button.GetComponentInChildren<TMP_Text>().text = "Add";
                break;
        }
    }

    public void CheckListGun()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        foreach (var button in addButtun)
        {
            TMP_Text text = button.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<TMP_Text>();
            if (player.GetComponent<PlayerCombat>().gunList.Count > 0)
            {
                foreach (var gun in player.GetComponent<PlayerCombat>().gunList)
                {
                    if (button.name == gun.name)
                    {
                        text.text = "Remove";
                        break;
                    }
                    else
                    {
                        text.text = "Add";
                    }
                }
            }
            
        }
        
    }

    public void OnClickPlayer(Button button)
    {
        string buttonName = button.name;
        int buttonNumber;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();

        if (int.TryParse(buttonName, out buttonNumber))
        {
            switch (text.text)
            {
                case "On":
                    onPlayer[buttonNumber] = true;
                    text.text = "Off";
                    break;
                case "Off":
                    onPlayer[buttonNumber] = false;
                    if (buttonNumber == 4)
                    {
                        RevertDamage();
                    }

                    text.text = "On";
                    break;
            }
        }
        else
        {
            Debug.LogError("Button name is not a valid integer: " + buttonName);
        }
       

        if (player = null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void PlayerImmortal()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        PlayerState state = player.GetComponent<PlayerState>();
        state.imortal = true;
    }

    public void UnlimitedEnergy()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        PlayerState playerState = player.GetComponent<PlayerState>();
        playerState.energy = playerState.maxEnergy;
    }

    public void UnlimitedUltimateEnergy()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        PlayerState playerState = player.GetComponent<PlayerState>();
        playerState.ultimateEnergy = playerState.maxUltimateEnergy;
    }

    public void UnlimitedDodgeCharge()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        movement.rollCharge = 0;
    }

    public void AllDamage1000()
    {
        BaseBullet[] foundBullets = GameObject.FindObjectsOfType<BaseBullet>();

        foreach (BaseBullet bullet in foundBullets)
        {
            if (!bullets.Contains(bullet) && bullet != null && bullet.tagUse == "Enemy")
            {
                bullets.Add(bullet);
                // เก็บค่า damage เดิมไว้
                originalDamages[bullet] = bullet.damage;
            }
        }

        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            if (bullets[i] == null || bullets[i].tagUse == "Player")
            {
                bullets.RemoveAt(i);
            }
        }

        foreach (var bullet in bullets)
        {
            bullet.damage = 1000;
        }
    }

    public void RevertDamage()
    {
        foreach (var bullet in bullets)
        {
            if (originalDamages.ContainsKey(bullet))
            {
                bullet.damage = originalDamages[bullet]; // คืนค่า damage เดิม
            }
        }
        originalDamages.Clear(); // ล้างข้อมูลหลังจากคืนค่า
        bullets.Clear(); // ล้างรายการกระสุนที่เก็บไว้
    }

    public void UnlimitedMaxAmmo()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
        if (playerCombat.gunList.Count > 0)
        {
            playerCombat.gunList[playerCombat.currentGun].ammo = playerCombat.gunList[playerCombat.currentGun].maxAmmo;
        }
    }

    public void CheckPlaystatus()
    {
        for (int i = 0; i < onPlayer.Length; i++)
        {
            if (onPlayer[i])
            {
                CasePlayer(i);
            }
        }
    }

    public void CasePlayer(int index)
    {
        if (player = null)
        {
            player = GameObject.Find("Player");
            Debug.Log("Get");
        }

        switch (index)
        {
            case 0:
                PlayerImmortal();
                break;
            case 1:
                UnlimitedEnergy();
                break;
            case 2:
                UnlimitedUltimateEnergy();
                break;
            case 3:
                UnlimitedDodgeCharge();
                break;
            case 4:
                AllDamage1000();
                break;
            case 5:
                UnlimitedMaxAmmo(); 
                break;
        }
    }

    public void NextPage()
    {
        curPage += 1 % maxPage;
    }

    public void BackPage()
    {
        curPage -= 1 % maxPage;
    }

    public void OnClickChangMode(int index)
    {
        Mode _mode = Mode.Spawnmon;
        switch (index)
        {
            case 1:
                _mode = Mode.Spawnmon;
                break;
            case 2:
                _mode = Mode.PlayerStatus;
                break;
            case 3:
                _mode = Mode.GunEdit;
                break;
        }

        ChangMode(_mode);
    }

    public void ChangMode(Mode _mode)
    {
        switch (mode)
        {
            case Mode.Spawnmon:
                curPageSpawn = curPage;
                allMenu[0].SetActive(false);
                break;
            case Mode.PlayerStatus:
                allMenu[1].SetActive(false);
                break;
            case Mode.GunEdit:
                curPageGun = curPage;
                allMenu[2].SetActive(false);
                break;
        }      

        mode = _mode;

        switch (mode)
        {
            case Mode.Spawnmon:
                allMenu[0].SetActive(true);
                break;
            case Mode.PlayerStatus:
                allMenu[1].SetActive(true);
                break;
            case Mode.GunEdit:
                allMenu[2].SetActive(true);
                break;
        }
    }


}
