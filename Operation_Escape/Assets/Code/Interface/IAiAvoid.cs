using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAiAvoid
{
    public GameObject playerGameObject { get; }
    public Transform targetTransform { get; set; }
    public Vector2 playerVelocity { get;}
    public Vector2 monVelocity { get; set; }
    public Vector3 destination { get; set; }
    public float maxspeed { get; set; }
    public float slowDownRadius { get; set; }
    public float stopRadius { get; set; }
    public bool stopRadiusOn { get; set; }
    public bool canMove { get; set; }
    public bool endMove { get;}
    public bool slowMove { get; }
    public Vector3 position { get; }
    public bool randomDeviation { get; set; }

}
