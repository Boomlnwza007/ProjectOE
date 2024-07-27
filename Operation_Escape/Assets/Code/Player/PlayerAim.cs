using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Transform aimTransform;
    private Camera mainCam;

    private void Awake()
    {
        aimTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 aimDir = (mousePos - aimTransform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;

        if (angle>90||angle<-90)
        {
            localScale.y = -1;
        }
        else
        {
            localScale.y = +1;
        }
        aimTransform.localScale = localScale;
    }
}
