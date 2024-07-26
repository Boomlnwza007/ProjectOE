using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform targert;
    public float smoothing = 0.1f;

    //public Vector2 Maxpos;
    //public Vector2 Minpos;

    private void Awake()
    {
        targert = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (transform.position != targert.position)
        {
            Vector3 targetPos = new Vector3(targert.position.x, targert.position.y, transform.position.z);
            //targetPos.x = Mathf.Clamp(targetPos.x, Minpos.x, Maxpos.x);
            //targetPos.y = Mathf.Clamp(targetPos.y, Minpos.y, Maxpos.y);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
        }
    }
}
