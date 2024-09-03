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

    [Header("GunEditer")]
    [SerializeField] private GameObject buttonPrefabGun;
    [SerializeField] private ID listGun;
    private List<GameObject> addButtun = new List<GameObject>();
    public Transform menuGun;
    private int curPageGun = 0;

    [Header("Other")]
    [SerializeField] private Canvas canvas;
    private GameObject player;
    public enum Mode { Spawnmon ,PlayerStatus ,GunEdit };
    [SerializeField] private GameObject[] allMenu;
    public Mode mode;
    private int curPage = 0;
    private int maxPage;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<IDamageable>().Takedamage(10, DamageType.Melee, 1);
        mode = Mode.Spawnmon;
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
        switch (mode)
        {
            case Mode.Spawnmon:
                CheckListMon();
                break;
            case Mode.PlayerStatus:
                break;
            case Mode.GunEdit:
                CheckListGun();
                break;
            default:
                break;
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
        if (player == null) // ถ้ายังไม่มีการกำหนดค่า
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        PlayerCombat playerCombat = player.GetComponent<PlayerCombat>(); 
        BaseGun gun = null;

        // Try to parse the button's name into an integer
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
        foreach (var button in addButtun)
        {
            TMP_Text text = button.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<TMP_Text>();
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
