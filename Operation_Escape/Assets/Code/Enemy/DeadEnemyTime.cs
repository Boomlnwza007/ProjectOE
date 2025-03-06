using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemyTime : DeadEnemy
{
    public float time = 5;
    private float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            spriteFlash.Flash();
            if (timer > time+0.2f)
            {
                Die();
            }
        }
    }
}
