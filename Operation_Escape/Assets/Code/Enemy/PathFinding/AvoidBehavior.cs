using System.Collections.Generic;
using UnityEngine;

public class AvoidBehavior : MonoBehaviour
{
    public Transform target; // �������
    public float speed = 5f; // �������Ǣͧ���¹��
    public float avoidRadius = 1f; // ����ա����ա����§
    public float rayLength = 1f; // ������Ǣͧ��� ray
    public float slowDownRadius = 5f; // ����ա�ê��ͤ�������
    public float stopRadius = 2f; // ����ա����ش
    public LayerMask obstacleLayer; // �������ͧ���¹����� � ����ͧ��ա����§
    public LayerMask agentLayer; // �������ͧ���¹����� � ����ͧ��ա����§

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

        // ���ѵ�ط���ͧ��ա����§
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, avoidRadius, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            Vector2 direction = (Vector2)transform.position - (Vector2)obstacle.transform.position;
            float distance = direction.magnitude;
            avoidForce += direction.normalized / distance;
        }

        // �����¹����� � ����ͧ��ա����§
        Collider2D[] agents = Physics2D.OverlapCircleAll(transform.position, avoidRadius, agentLayer);
        foreach (var agent in agents)
        {
            if (agent.gameObject != gameObject) // �����ա����§����ͧ
            {
                Vector2 direction = (Vector2)transform.position - (Vector2)agent.transform.position;
                float distance = direction.magnitude;
                avoidForce += direction.normalized / distance;
            }
        }

        // �ӹǳ��ȷҧ��ѧ�������
        Vector2 targetDirection = (Vector2)target.position - (Vector2)transform.position;
        float distanceToTarget = targetDirection.magnitude;

        // ���ͤ�������������������������
        curSpeed = speed;
        if (distanceToTarget < stopRadius)
        {
            curSpeed = 0f;
        }
        else if (distanceToTarget < slowDownRadius)// ��ش����Ͷ֧�������
        {
            curSpeed = Mathf.Lerp(0, speed, distanceToTarget / slowDownRadius);
        }

        Vector2 desiredVelocity = targetDirection.normalized;
        // ����ç��ա����§�Ѻ�������Ƿ���ͧ���
        steering = desiredVelocity + avoidForce;

        rb.velocity = steering.normalized * curSpeed;

        Vector2 ForwardObtrac = Vector2.zero;
        // �ԧ��� ray ������������ѵ�آ�ҧ˹���������
        RaycastHit2D hitForward = Physics2D.Raycast(transform.position, rb.velocity.normalized, rayLength, obstacleLayer);
        if (hitForward.collider != null)
        {
            // ������ѵ�آ�ҧ˹�� ����Ѻ��ȷҧ
            Vector2 newDirection = Vector2.Perpendicular(rb.velocity).normalized;
            rb.velocity = newDirection * curSpeed;  
        }
        Vector2 LRObtrac = Vector2.zero;
        // �ԧ��� ray ��ҹ��ҧ������ա����§���䶵��
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 45) * rb.velocity.normalized, rayLength, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -45) * rb.velocity.normalized, rayLength, obstacleLayer);

        if (hitLeft.collider != null || hitRight.collider != null)
        {
            // ������ѵ�ش�ҹ��ҧ ����Ѻ��ȷҧ
            Vector2 newDirection = (hitLeft.collider != null) ? Quaternion.Euler(0, 0, -45) * rb.velocity.normalized : Quaternion.Euler(0, 0, 45) * rb.velocity.normalized;
            rb.velocity = newDirection * curSpeed;
        }
        
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);

        // �Ҵ��� ray ����Ѻ debugging
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized * rayLength);

        // �Ҵ��� ray ��ҹ��ҧ
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Quaternion.Euler(0, 0, 45) * rb.velocity.normalized * rayLength));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Quaternion.Euler(0, 0, -45) * rb.velocity.normalized * rayLength));

        // �Ҵ����ժ��ͤ������������ش
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, slowDownRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stopRadius);
    }
}
