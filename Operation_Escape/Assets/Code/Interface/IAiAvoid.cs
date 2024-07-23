using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAiAvoid
{
    Transform target { get; set; }
    float speed { get; set; }
    bool canMove { get; set; }
    bool endMove { get; set; }

}
