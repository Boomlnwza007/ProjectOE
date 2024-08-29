using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Camera mainCam;
    //[SerializeField] private Transform body;
    public float angle;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float _angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle = _angle;
        Vector3 localScaleGun = Vector3.one;
        //Vector3 localScaleBody = Vector3.one;

        if (_angle > 90 || _angle < -90)
        {
            localScaleGun.y = -1;
            //localScaleBody.x = -1;
            //_angle += 180f;
        }
        else
        {
            localScaleGun.y = +1;
            //localScaleBody.x = +1;
        }

        transform.eulerAngles = new Vector3(0, 0, _angle);
        transform.localScale = localScaleGun;
        //body.localScale = localScaleBody;
    }

}
