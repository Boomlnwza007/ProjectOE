using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDrop : MonoBehaviour
{
    [SerializeField]public BaseGun gun;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerCombat>().Addgun(gun);
            Destroy(gameObject);
        }
    }
}
