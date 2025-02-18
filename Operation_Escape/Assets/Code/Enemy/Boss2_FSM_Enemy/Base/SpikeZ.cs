using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeZ : MonoBehaviour
{
    public float time;
    public float speed = 5f;
    private float timer = 0;
    public Transform player;
    public GameObject Spike;
    public static bool hit;
    public static bool end;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (timer < time)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    public static void Setup()
    {
        hit = false;
        end = false;
    }

    public void Destroy()
    {        
        Destroy(gameObject);
        end = true;
    }

    public void NoHitDestroy()
    {
        if (!hit)
        {
            Destroy(gameObject);
            end = true;
        }

    }

    public void SpawnParticle(float radius)
    {
        int numObjects = Mathf.FloorToInt((2 * Mathf.PI * radius) / 2); ;
        for (int i = 0; i < numObjects; i++)
        {
            float angle = i * Mathf.PI * 2f / numObjects; // คำนวณมุมของแต่ละจุด
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 spawnPosition = new Vector3(x, y, 0) + transform.position;

            Instantiate(Spike, spawnPosition, Quaternion.identity,transform);
        }
    }
}
