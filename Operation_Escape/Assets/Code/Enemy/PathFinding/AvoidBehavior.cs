using System.Collections.Generic;
using UnityEngine;

public class AvoidBehavior : MonoBehaviour
{
    public Transform target; // เป้าหมาย
    public float speed = 5f; // ความเร็วของเอเยนต์
    public float avoidRadius = 1f; // รัศมีการหลีกเลี่ยง
    public float rayLength = 1f; // ความยาวของเส้น ray
    public float slowDownRadius = 5f; // รัศมีการชะลอความเร็ว
    public float stopRadius = 2f; // รัศมีการหยุด
    public LayerMask obstacleLayer; // เลเยอร์ของเอเยนต์อื่น ๆ ที่ต้องหลีกเลี่ยง
    public LayerMask agentLayer; // เลเยอร์ของเอเยนต์อื่น ๆ ที่ต้องหลีกเลี่ยง

    [SerializeField]private Rigidbody2D rb;
    private Vector2 steering;
    private float curSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        

        
    }

    private void FixedUpdate()
    {
        Vector2 avoidForce = Vector2.zero;

        // หาวัตถุที่ต้องหลีกเลี่ยง
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, avoidRadius, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            Vector2 direction = (Vector2)transform.position - (Vector2)obstacle.transform.position;
            float distance = direction.magnitude;
            avoidForce += direction.normalized / distance;
        }

        // หาเอเยนต์อื่น ๆ ที่ต้องหลีกเลี่ยง
        Collider2D[] agents = Physics2D.OverlapCircleAll(transform.position, avoidRadius, agentLayer);
        foreach (var agent in agents)
        {
            if (agent.gameObject != gameObject) // ไม่หลีกเลี่ยงตัวเอง
            {
                Vector2 direction = (Vector2)transform.position - (Vector2)agent.transform.position;
                float distance = direction.magnitude;
                avoidForce += direction.normalized / distance;
            }
        }

        // คำนวณทิศทางไปยังเป้าหมาย
        Vector2 targetDirection = (Vector2)target.position - (Vector2)transform.position;
        float distanceToTarget = targetDirection.magnitude;

        // ชะลอความเร็วเมื่อเข้าใกล้เป้าหมาย
        curSpeed = speed;
        if (distanceToTarget < stopRadius)
        {
            curSpeed = 0f;
        }
        else if (distanceToTarget < slowDownRadius)// หยุดเมื่อถึงเป้าหมาย
        {
            curSpeed = Mathf.Lerp(0, speed, distanceToTarget / slowDownRadius);
        }

        Vector2 desiredVelocity = targetDirection.normalized;
        // รวมแรงหลีกเลี่ยงกับความเร็วที่ต้องการ
        steering = desiredVelocity + avoidForce;

        rb.velocity = steering.normalized * curSpeed;

        Vector2 ForwardObtrac = Vector2.zero;
        // ยิงเส้น ray เพื่อเช็คว่ามีวัตถุข้างหน้าหรือไม่
        RaycastHit2D hitForward = Physics2D.Raycast(transform.position, rb.velocity.normalized, rayLength, obstacleLayer);
        if (hitForward.collider != null)
        {
            // ถ้ามีวัตถุข้างหน้า ให้ปรับทิศทาง
            Vector2 newDirection = Vector2.Perpendicular(rb.velocity).normalized;
            rb.velocity = newDirection * curSpeed;  
        }
        Vector2 LRObtrac = Vector2.zero;
        // ยิงเส้น ray ด้านข้างเพื่อหลีกเลี่ยงการไถตัว
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 45) * rb.velocity.normalized, rayLength, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -45) * rb.velocity.normalized, rayLength, obstacleLayer);

        if (hitLeft.collider != null || hitRight.collider != null)
        {
            // ถ้ามีวัตถุด้านข้าง ให้ปรับทิศทาง
            Vector2 newDirection = (hitLeft.collider != null) ? Quaternion.Euler(0, 0, -45) * rb.velocity.normalized : Quaternion.Euler(0, 0, 45) * rb.velocity.normalized;
            rb.velocity = newDirection * curSpeed;
        }
        
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);

        // วาดเส้น ray สำหรับ debugging
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized * rayLength);

        // วาดเส้น ray ด้านข้าง
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Quaternion.Euler(0, 0, 45) * rb.velocity.normalized * rayLength));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Quaternion.Euler(0, 0, -45) * rb.velocity.normalized * rayLength));

        // วาดรัศมีชะลอความเร็วและหยุด
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, slowDownRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stopRadius);
    }
}
