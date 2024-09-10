using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGun : MonoBehaviour
{
    [SerializeField] private Transform spawnBar;
    public ID obj;
    [HideInInspector] public int curgun;
    public GameObject bar;

    public void ChangPrefab()
    {
        if (spawnBar.childCount > 0)
        {
            foreach (Transform child in spawnBar.transform)
            {
                Destroy(child.gameObject);
            }
        }

        Instantiate(obj.Item[curgun], spawnBar);
    }

    public void SetUp(string name)
    {
        for (int i = 0; i < obj.nameItem.Length; i++)
        {
            if (name == obj.nameItem[i])
            {
                curgun = i;
            }
        }

        ChangPrefab();
    }

    public void Off()
    {
        bar.SetActive(false);
    }
}
