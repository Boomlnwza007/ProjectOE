using System.Collections.Generic;
using UnityEngine;

public class ReflectingRay : MonoBehaviour
{

    public LayerMask obstacleLayer;
    public Transform playerTest;
    public float width;
    public float height;
    public int gridSize;

    [ContextMenu("Check")]
    public void Test()
    {
        CheckObjectsInArea(playerTest.position, width, height, gridSize, obstacleLayer);
    }

    public List<Vector3> CheckObjectsInArea(Vector2 center, float width, float height, int gridSize, LayerMask layerMask)
    {
        float gridWidth = width / gridSize;  // ความกว้างของแต่ละช่อง
        float gridHeight = height / gridSize; // ความสูงของแต่ละช่อง
        List<Vector3> vectors = new List<Vector3>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // ปรับให้ `center` เป็นศูนย์กลางของตารางแทนที่จะเป็นมุมซ้ายล่าง
                Vector2 checkPosition = center + new Vector2(
                    (x - (gridSize - 1) / 2f) * gridWidth,
                    (y - (gridSize - 1) / 2f) * gridHeight
                );

                Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPosition, new Vector2(gridWidth, gridHeight), 0f, layerMask);

                // ถ้าไม่มีวัตถุให้เพิ่มใน List และวาดเส้นเป็น "สีเขียว"
                Color debugColor = colliders.Length == 0 ? Color.green : Color.red;

                if (colliders.Length == 0)
                {
                    vectors.Add(checkPosition);
                }

                // วาดตาราง Debug
                DrawDebugBox(checkPosition, gridWidth, gridHeight, debugColor);
            }
        }

        return vectors;
    }

    // ฟังก์ชันสำหรับวาด Debug Box
    private void DrawDebugBox(Vector2 center, float width, float height, Color color)
    {
        Vector3 bottomLeft = center - new Vector2(width / 2, height / 2);
        Vector3 bottomRight = center + new Vector2(width / 2, -height / 2);
        Vector3 topLeft = center + new Vector2(-width / 2, height / 2);
        Vector3 topRight = center + new Vector2(width / 2, height / 2);

        Debug.DrawLine(bottomLeft, bottomRight, color, 5f);
        Debug.DrawLine(bottomRight, topRight, color, 5f);
        Debug.DrawLine(topRight, topLeft, color, 5f);
        Debug.DrawLine(topLeft, bottomLeft, color, 5f);
    }
}
