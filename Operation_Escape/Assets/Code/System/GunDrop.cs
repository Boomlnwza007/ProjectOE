using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDrop : MonoBehaviour
{
    [SerializeField]public BaseGun gun;
    public float floatAmplitude = 0.5f; // ความสูงของการลอย
    public float floatSpeed = 1f; // ความเร็วในการลอย
    public bool fake;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position; // เก็บตำแหน่งเริ่มต้น
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Instantiate(gun.gunPrefab, gameObject.transform);
    }

    private void FixedUpdate()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerCombat>().Addgun(gun, fake);
            Destroy(gameObject);
        }
    }
}
