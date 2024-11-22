using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckPoint : MonoBehaviour
{
    public Transform checkPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PauseScene.spawnPoint = checkPoint;
            checkPoint.gameObject.GetComponentInChildren<SavePoint>().SetAc(true);
        }
    }
}
