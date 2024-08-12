using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAiAvoid
{
    public Transform target { get; set; }
    public Vector3 destination { get; set; }
    public float Maxspeed { get; set; }
    public bool canMove { get; set; }
    public bool endMove { get;}
    public bool slowMove { get; }
    public Vector3 position { get; }
    public bool randomDeviation { get; set; }

}
