using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemyBoss2 : DeadEnemy
{
    public Boss2Mark boss2Mark;

    private void Awake()
    {
        boss2Mark = GameObject.Find("Boss2Mark").GetComponent<Boss2Mark>();

    }

    public void SetActive()
    {
        boss2Mark.SetActive();
    }
}
