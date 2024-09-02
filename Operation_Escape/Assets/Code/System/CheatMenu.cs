using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatMenu : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private ID listenemy;
    private GameObject player;
    private List<GameObject> enemySpawnNow = new List<GameObject>();
    private int page = 0;
    public Transform menu;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<IDamageable>().Takedamage(10, DamageType.Melee, 1);
    }

    void Start()
    {
        canvas.worldCamera = Camera.main;
        if (listenemy != null)
        {
            for (int i = 0; i < listenemy.Item.Length; i++)
            {
                GameObject button = Instantiate(buttonPrefab, menu);
                button.name = i.ToString();
                button.GetComponentInChildren<TMP_Text>().text = listenemy.Item[i].name;
            }
        }
    }

    private void Update()
    {
        CheckList();
    }

    public void OnClick(Button button)
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

    public void CheckList()
    {
        for (int i = 0; i < enemySpawnNow.Count; i++)
        {
            if (enemySpawnNow[i] == null)
            {
                enemySpawnNow.RemoveAt(i);
            }
        }
    }

}
