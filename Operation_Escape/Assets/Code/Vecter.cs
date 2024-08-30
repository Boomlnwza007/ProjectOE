using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecter : MonoBehaviour
{
    public Transform hand;
    public Transform target;

    public void MeleeFollow()
    {
        Vector2 dir = (target.position - hand.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = hand.eulerAngles.z;
        targetAngle += 45;
        targetAngle = (targetAngle + 360) % 360;
        int segment = Mathf.FloorToInt(targetAngle / 90);

        float newAngle = Mathf.LerpAngle(currentAngle, segment * 90, Time.deltaTime * 30 * 2);
        hand.eulerAngles = new Vector3(0, 0, newAngle);


        //Debug.Log(angle);
        // hand.eulerAngles = new Vector3(0, 0, segment * 90);

    }

    private void Update()
    {
        MeleeFollow();
    }

    private void OnDrawGizmos()
    {
       
    }
}
