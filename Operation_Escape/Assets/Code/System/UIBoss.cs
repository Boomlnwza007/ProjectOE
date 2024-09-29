using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIBoss : MonoBehaviour
{
    public static UIBoss uiBoss;
    public GameObject uiBody;
    public TMP_Text text;
    public SliderBar slider;

    private void Awake()
    {
        uiBoss = this;
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
