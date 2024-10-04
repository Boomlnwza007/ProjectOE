using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIBoss : MonoBehaviour
{
    public FSMBoss1EnemySM boss;
    public GameObject uiBody;
    public TMP_Text text;
    public SliderBar slider;    

    private void Awake()
    {
        slider.SetMax(boss.maxHealth);
    }

    private void Update()
    {
        if (boss.Health != slider.value)
        {
            slider.SetValue(boss.Health);
        }
    }

    public void SetUpBoss(string nameBoss, int hpBoss)
    {
        text.text = nameBoss;
        slider.SetMax(hpBoss);
    }

    public void SetUiBody(bool on) 
    {
        uiBody.SetActive(on);
    }
}
