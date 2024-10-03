using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoss : MonoBehaviour
{
    [SerializeField] private StateMachine Boss;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Boss.attacking = true;
    }
}
