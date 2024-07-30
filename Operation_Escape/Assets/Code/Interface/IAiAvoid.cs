using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAiAvoid
{
    public Transform target { get; set; }
    public float Maxspeed { get; set; }
    public bool canMove { get; set; }
    public bool endMove { get;}
    Vector3 position { get; }

}
