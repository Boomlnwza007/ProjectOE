using System.Collections.Generic;
using UnityEngine;

public class AvoidBehavior : MonoBehaviour, IAiAvoid
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
    public Transform targetTarnsform { get; set; }
    public bool randomDeviation { get { return enableRandomDeviation; } set { enableRandomDeviation = value; } }
    public GameObject playerGameObject { get; private set; }
    public Vector2 playerVelocity { get; private set; }

    public float slowDownRadius = 5f;
    public float stopRadius = 2f;
    public bool enableRandomDeviation = true;
    public float deviationAngle = 5f; // ลดมุมเบี่ยงเบน
    public float deviationChangeInterval = 2f;
    private float deviationTimer;
    private float currentDeviationAngle;

    [SerializeField] private Rigidbody2D rb;
    private Vector2 steering;
    public float curSpeed;
    private Vector2 velocity = Vector2.zero;

    private void Awake()
    {
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerVelocity = playerGameObject.GetComponent<Rigidbody2D>().velocity;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        deviationTimer = deviationChangeInterval;
        currentDeviationAngle = Random.Range(-deviationAngle, deviationAngle);

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

        endMove = false;
        slowMove = false;

        if (distanceToTarget < stopRadius)
        {
            curSpeed = 0f;
            endMove = true;
        }
        else if (distanceToTarget < slowDownRadius)
        {
            slowMove = true;
            curSpeed = Mathf.Lerp(0, speed, distanceToTarget / slowDownRadius);
        }

        deviationTimer -= Time.deltaTime;
        if (deviationTimer <= 0f)
        {
            currentDeviationAngle = Random.Range(-deviationAngle, deviationAngle);
            deviationTimer = deviationChangeInterval;
        }

        Vector2 desiredVelocity = targetDirection.normalized;

        if (enableRandomDeviation && avoidForce == Vector2.zero)
        {
            desiredVelocity = Quaternion.Euler(0, 0, currentDeviationAngle) * desiredVelocity;
        }

        steering = desiredVelocity + avoidForce;

        if (!canMove)
        {
            curSpeed = 0f;
        }

        if (avoidForce != Vector2.zero)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, steering.normalized * curSpeed, ref velocity, smoothTime);
        }
        else
        {
            rb.velocity = steering.normalized * curSpeed;
        }

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