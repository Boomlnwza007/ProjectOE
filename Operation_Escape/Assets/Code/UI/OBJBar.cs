using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJBar : MonoBehaviour
{
    [SerializeField] private Transform spawnBar;
    [HideInInspector] public int value;
    public ID obj;
    public ID objUl;
    [HideInInspector]public int curgun;
    public GameObject bar;

    private void Update()
    {
        if (spawnBar.childCount != value)
        {
            Chang();
        }
    }

    public void SetUp(string name , bool Ul)
    {
        for (int i = 0; i < obj.nameItem.Length; i++)
        {
            if (name == obj.nameItem[i])
            {
                curgun = i;
            }
        }

        ChangPrefab(Ul);
    }

    public void ChangPrefab(bool Ul)
    {
        foreach (Transform child in spawnBar.transform)
        {
            Destroy(child.gameObject);
        }

        var itemList = Ul ? objUl.Item : obj.Item;
        for (int i = 0; i < value; i++)
        {
            Instantiate(itemList[curgun], spawnBar);
        }
    }

    public void Chang()
    {
        if (spawnBar.childCount > value)
        {
            Destroy(spawnBar.GetChild(spawnBar.childCount-1).gameObject);
        }
        else
        {
           Instantiate(obj.Item[curgun], spawnBar);
        }
    }   

    public void SetValue(int _value)
    {
        value = _value;
    }   
}
