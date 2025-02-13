using System.Collections.Generic;
using UnityEngine;

public class ReflectingRay : MonoBehaviour
{
    public float retreatSpeed = 5f; // ความเร็วในการถอยหลัง
    public float detectionRadius = 10f; // ระยะที่ตรวจจับผู้เล่น
    public LayerMask obstacleLayer; // เลเยอร์สำหรับกำแพงหรืออุปสรรค
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 directionToPlayer = (transform.position - player.position).normalized; // คำนวณทิศทางที่มอนสเตอร์จะถอยไป
        Vector2 retreatDirection = GetRetreatDirection(directionToPlayer);

        // เคลื่อนที่ถอยหลังไปในทิศทางที่ไม่ชนกำแพง
        rb.velocity = retreatDirection * retreatSpeed;
    }

    Vector2 GetRetreatDirection(Vector2 directionToPlayer)
    {
        Vector2 newDirection = directionToPlayer;

        // ตรวจสอบว่าทิศทางนี้ชนกำแพงหรือไม่
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, 1f, obstacleLayer);

        if (hit.collider != null) // ถ้าชนกำแพง
        {
            // ลองหาทิศทางใหม่ที่ไม่ชนกำแพง
            newDirection = GetAvoidanceDirection(directionToPlayer);
        }

        return newDirection;
    }

    Vector2 GetAvoidanceDirection(Vector2 directionToPlayer)
    {
        Vector2 leftDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x).normalized;
        Vector2 rightDirection = new Vector2(directionToPlayer.y, -directionToPlayer.x).normalized;

        // ตรวจสอบทิศทางซ้าย
        if (!IsBlocked(leftDirection))
        {
            return leftDirection;
        }

        // ตรวจสอบทิศทางขวา
        if (!IsBlocked(rightDirection))
        {
            return rightDirection;
        }

        // ถ้าทั้งสองทิศทางไม่ว่าง ก็ค่อยกลับไปใช้ทิศทางเดิม
        return directionToPlayer;
    }

    bool IsBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
        return hit.collider != null; // ถ้าชนกำแพงให้ return true
    }
}
