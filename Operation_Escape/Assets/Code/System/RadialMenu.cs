using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    private Camera mainCam;
    public Transform center;
    public Transform selectObject;
    public float offset;

    public GameObject Wheel;

    bool isMenuActive;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        isMenuActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Wheel"))
        {
            Wheel.SetActive(true);
            isMenuActive = true;
        }

        if (Input.GetButtonUp("Wheel"))
        {
            Wheel.SetActive(false);
            isMenuActive = false;
        }

        if (isMenuActive)
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector2 delta = mousePos - center.position;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            angle += offset;
            angle = (angle + 360) % 360;
            Debug.Log(angle);
            selectObject.rotation = Quaternion.Euler(0, 0, angle);
            //int segment = Mathf.FloorToInt(angle / 60); 
            //selectObject.rotation = Quaternion.Euler(0, 0, segment * 60);
        }
    }
}
