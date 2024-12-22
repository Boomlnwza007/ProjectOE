using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckPoint : MonoBehaviour
{
    public Transform checkPoint;
    private bool active = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
        {
            if (collision.CompareTag("Player"))
            {
                //PauseScene.spawnPoint?.GetComponentInChildren<SavePoint>()?.SetAc(false);
                PauseScene.spawnPoint = checkPoint;
                checkPoint.gameObject?.GetComponentInChildren<SavePoint>()?.SetAc(true);
                foreach (var area in AreaEnermy.area)
                {
                    area.ready = false;
                    area.ForceLock();
                }
                AreaEnermy.area.Clear();
                active = false;
            }
        }
    }
}
