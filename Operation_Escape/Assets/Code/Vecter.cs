using System.Collections.Generic;
using UnityEngine;

public class ReflectingRay : MonoBehaviour
{
    public float retreatSpeed = 5f; // ��������㹡�ö����ѧ
    public float detectionRadius = 10f; // ���з���Ǩ�Ѻ������
    public LayerMask obstacleLayer; // �����������Ѻ��ᾧ�����ػ��ä
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 directionToPlayer = (transform.position - player.position).normalized; // �ӹǳ��ȷҧ����͹�����ж���
        Vector2 retreatDirection = GetRetreatDirection(directionToPlayer);

        // ����͹�������ѧ�㹷�ȷҧ�����誹��ᾧ
        rb.velocity = retreatDirection * retreatSpeed;
    }

    Vector2 GetRetreatDirection(Vector2 directionToPlayer)
    {
        Vector2 newDirection = directionToPlayer;

        // ��Ǩ�ͺ��ҷ�ȷҧ��骹��ᾧ�������
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, 1f, obstacleLayer);

        if (hit.collider != null) // ��Ҫ���ᾧ
        {
            // �ͧ�ҷ�ȷҧ��������誹��ᾧ
            newDirection = GetAvoidanceDirection(directionToPlayer);
        }

        return newDirection;
    }

    Vector2 GetAvoidanceDirection(Vector2 directionToPlayer)
    {
        Vector2 leftDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x).normalized;
        Vector2 rightDirection = new Vector2(directionToPlayer.y, -directionToPlayer.x).normalized;

        // ��Ǩ�ͺ��ȷҧ����
        if (!IsBlocked(leftDirection))
        {
            return leftDirection;
        }

        // ��Ǩ�ͺ��ȷҧ���
        if (!IsBlocked(rightDirection))
        {
            return rightDirection;
        }

        // ��ҷ���ͧ��ȷҧ�����ҧ ����¡�Ѻ����ȷҧ���
        return directionToPlayer;
    }

    bool IsBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
        return hit.collider != null; // ��Ҫ���ᾧ��� return true
    }
}
