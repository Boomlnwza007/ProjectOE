using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckPoint : MonoBehaviour
{
    public Transform checkPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PauseScene.spawnPoint = checkPoint;
        Destroy(gameObject);
    }
}
