using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecter : MonoBehaviour
{
    public int nLineRay = 17;
    public float angle = 90f;
    public float rayRang = 17;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float detectionDistance = 2f;

    private void OnDrawGizmos()
    {
        //int c = (int)rayRang-nLineRay/2;
        //bool pn = false;
        //for (int i = 0; i < nLineRay; i++)
        //{
        //    var rotation = this.transform.rotation;
        //    var rotationMod = Quaternion.AngleAxis(i / ((float)nLineRay - 1) * angle * 2 - angle, Vector3.forward);
        //    var dir = rotation * rotationMod * Vector2.up;
        //    Gizmos.DrawLine(this.transform.position, dir.normalized * c);
        //    if (c>=rayRang || pn)
        //    {
        //        c--;
        //        pn = true;
        //    }
        //    else
        //    {
        //        c++;
        //    }

        //}
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * detectionDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.right + Vector2.up).normalized * detectionDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.right + Vector2.down).normalized * detectionDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.right + Vector2.up * 0.5f).normalized * detectionDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2.right + Vector2.down * 0.5f).normalized * detectionDistance);
        //var facing = gameObject.transform.up;
        //Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 30.0f);
        //Vector3 direction = rotation * facing;
        //Gizmos.DrawLine(this.transform.position, this.transform.position+ facing);
        //Gizmos.DrawLine(this.transform.position, this.transform.position+direction.normalized );


    }
}
