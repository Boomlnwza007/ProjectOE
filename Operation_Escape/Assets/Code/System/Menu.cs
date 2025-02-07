using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingMenu;

    public void Show()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        settingMenu.SetActive(!settingMenu.activeSelf);
    }

    public void RollBack()
    {
        mainMenu.SetActive(true);
        settingMenu.SetActive(false);
    }
}
