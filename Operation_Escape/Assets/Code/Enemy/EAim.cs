using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAim : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
