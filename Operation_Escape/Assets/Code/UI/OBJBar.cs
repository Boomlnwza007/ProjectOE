using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJBar : MonoBehaviour
{
    [SerializeField]private Transform spawnBar;
    public int value;
    public GameObject obj;
    public GameObject bar;
    private void Update()
    {
        if (spawnBar.childCount != value)
        {
            Chang();
        }
    }

    public void SetUp(int _value)
    {
        if (spawnBar.childCount > 0)
        {
            foreach (Transform child in spawnBar.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < _value; i++)
        {
            Instantiate(obj, spawnBar);
        }
    }

    public void Chang()
    {
        Debug.Log(spawnBar.childCount + " ");
        if (spawnBar.childCount > value)
        {
            Destroy(spawnBar.GetChild(spawnBar.childCount-1).gameObject);
        }
        else
        {
            GameObject newObj = Instantiate(obj, spawnBar);
        }
    }

    public void SetValue(int _value)
    {
        value = _value;
    }

    public void Off()
    {
        bar.SetActive(false);
    }
}
