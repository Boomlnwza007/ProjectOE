using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetKey : MonoBehaviour
{
    [SerializeField] private int key = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControl.control.key.Add(key);
        Destroy(gameObject);
    }
}
