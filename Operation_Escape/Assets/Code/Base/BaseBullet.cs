using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public int damage;
    public float speed;
    public float knockbackForce;
    public string tagUse;
    public Transform target;
    public bool ready;
    public virtual void ResetGameObj() {}

    public void KnockBackPush(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackDirection = collision.transform.position - transform.position;
            knockbackDirection.Normalize();

            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    public Vector2 PredictiveAim(Vector2 targetPosition, Vector2 playerVelocity)
    {
        float bulletTimeToTarget = Vector2.Distance(transform.position, targetPosition) / speed;
        Vector2 predictedTargetPosition = targetPosition + (playerVelocity * bulletTimeToTarget);
        return predictedTargetPosition;
    }

    public Vector2 Direction()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector2 predictedTarget = PredictiveAim(mousePosition, PlayerControl.control.playerMovement.rb.velocity);
        Vector2 direction = (predictedTarget - (Vector2)transform.position).normalized;
        return direction;
    }

    public void SpriteRotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;        
        gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
