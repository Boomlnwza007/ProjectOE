using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeScene : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ControlScene.scene.ChangeScene(0);
        }
    }
}
