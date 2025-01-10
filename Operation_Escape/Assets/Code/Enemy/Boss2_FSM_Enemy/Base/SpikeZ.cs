using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeZ : MonoBehaviour
{
    public float time;
    private float timer = 0;
    public Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (timer < time)
        {
            timer += Time.deltaTime;
            transform.position = player.position;
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
