using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Transform aimTransform;
    private Camera mainCam;
    public float angle;

    private void Awake()
    {
        aimTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        mainCam = Camera.main;
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 aimDir = (mousePos - aimTransform.position).normalized;
        float _angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, _angle);
        Vector3 localScale = Vector3.one;

        angle = _angle;
        if (_angle>90||_angle<-90)
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
