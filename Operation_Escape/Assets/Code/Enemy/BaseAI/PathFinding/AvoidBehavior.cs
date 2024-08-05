using System.Collections.Generic;
using UnityEngine;

public class AvoidBehavior : MonoBehaviour ,IAiAvoid
{
    public float avoidRadius = 1f;
    public float rayLength = 1f;
    public LayerMask obstacleLayer;
    public LayerMask agentLayer;
    public float smoothTime = 0.3f;
    public float speed = 5f;
    public float Maxspeed { get { return speed; } set { speed = value; } }
    public bool canMove { get; set; }
    public bool slowMove { get; private set; }
    public bool endMove { get; private set; }
    public Vector3 position { get { return gameObject.transform.position; } }
    public Vector3 destination { get; set; }
    public Transform target { get; set; }

    public float slowDownRadius = 5f;
    public float stopRadius = 2f;

    [SerializeField]private Rigidbody2D rb;
    private Vector2 steering;
    private float curSpeed;
    private Vector2 velocity = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
    }

    private void FixedUpdate()
    {
        Move();
    }   

    public void Move()
    {
        Vector2 avoidForce = Vector2.zero;

        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, avoidRadius, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            Vector2 direction = (Vector2)transform.position - (Vector2)obstacle.transform.position;
            float distance = direction.magnitude;
            avoidForce += direction.normalized / distance;
        }

        Collider2D[] agents = Physics2D.OverlapCircleAll(transform.position, avoidRadius, agentLayer);
        foreach (var agent in agents)
        {
            if (agent.gameObject != gameObject)
            {
                Vector2 direction = (Vector2)transform.position - (Vector2)agent.transform.position;
                float distance = direction.magnitude;
                avoidForce += direction.normalized / distance;
            }
        }

        Vector2 targetDirection = (Vector2)destination - (Vector2)transform.position;
        float distanceToTarget = targetDirection.magnitude;

        curSpeed = speed;

        if (!canMove)
        {
            curSpeed = 0f;
        }

        endMove = false;
        slowMove = false;

        if (distanceToTarget < stopRadius)
        {
            curSpeed = 0f;
            endMove = true;
        }
        else if (distanceToTarget < slowDownRadius)// หยุดเมื่อถึงเป้าหมาย
        {
            slowMove = true;
            curSpeed = Mathf.Lerp(0, speed, distanceToTarget / slowDownRadius);
        }

        Vector2 desiredVelocity = targetDirection.normalized;
        steering = desiredVelocity + avoidForce;

        rb.velocity = Vector2.SmoothDamp(rb.velocity, steering.normalized * curSpeed, ref velocity, smoothTime);

        RaycastHit2D hitForward = Physics2D.Raycast(transform.position, rb.velocity.normalized, rayLength, obstacleLayer);
        if (hitForward.collider != null)
        {
            Vector2 newDirection = Vector2.Perpendicular(rb.velocity).normalized;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, newDirection * curSpeed, ref velocity, smoothTime);
        }

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 45) * rb.velocity.normalized, rayLength, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -45) * rb.velocity.normalized, rayLength, obstacleLayer);

        if (hitLeft.collider != null || hitRight.collider != null)
        {
            Vector2 newDirection = (hitLeft.collider != null) ? Quaternion.Euler(0, 0, -45) * rb.velocity.normalized : Quaternion.Euler(0, 0, 45) * rb.velocity.normalized;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, newDirection * curSpeed, ref velocity, smoothTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized * rayLength);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Quaternion.Euler(0, 0, 45) * rb.velocity.normalized * rayLength));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Quaternion.Euler(0, 0, -45) * rb.velocity.normalized * rayLength));

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, slowDownRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stopRadius);
    }
}
