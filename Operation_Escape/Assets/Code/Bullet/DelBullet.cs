using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelBullet : MonoBehaviour
{
    public GameObject delObj;

    public void Destroy()
    {
        Destroy(delObj);
    }
}
