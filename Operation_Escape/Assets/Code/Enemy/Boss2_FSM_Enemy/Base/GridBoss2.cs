using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoss2 : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    private bool[,] grid;
    private List<Vector2Int> spawnnPoint = new List<Vector2Int>();
    public Transform spawnParent;
    public LayerMask raycastMask;


    void Start()
    {
        grid = new bool[gridWidth, gridHeight];

        CheckGridObstacles();
    }

    public void SpawnSpike(Spike spikeType)
    {
        Vector2Int? spawnPosition = FindAvailablePosition(spikeType.size);

        if (spawnPosition != null)
        {
            Vector2Int pos = spawnPosition.Value;
            Vector3 worldPos = GridToWorldPosition(pos);

            Spike tower = Instantiate(spikeType.gameObject, worldPos, Quaternion.identity, spawnParent).GetComponent<Spike>();
            tower.Setup(this, pos);

            MarkGrid(pos, spikeType.size, true);
        }
        else
        {
            Debug.Log("No available space for this tower!");
        }
    }

    private Vector2Int? FindAvailablePosition(Vector2Int size)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        for (int x = 0; x < gridWidth - size.x + 1; x++)
        {
            for (int y = 0; y < gridHeight - size.y + 1; y++)
            {
                if (CanPlaceTower(new Vector2Int(x, y), size))
                {
                    possiblePositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (possiblePositions.Count > 0)
        {
            int randomIndex = Random.Range(0, possiblePositions.Count);
            return possiblePositions[randomIndex];
        }

        return null;
    }

    private bool CanPlaceTower(Vector2Int start, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (grid[start.x + x, start.y + y]) return false;
            }
        }
        return true;
    }

    public void MarkGrid(Vector2Int start, Vector2Int size, bool isOccupied)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[start.x + x, start.y + y] = isOccupied;
            }
        }
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return transform.position + new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }

    private void CheckGridObstacles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = GridToWorldPosition(new Vector2Int(x, y));

                Collider2D collider = Physics2D.OverlapBox(worldPos, new Vector2(cellSize, cellSize), 0, raycastMask);

                if (collider != null)
                {
                    grid[x, y] = true;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (gridWidth <= 0 || gridHeight <= 0 || cellSize <= 0) return;

        Gizmos.color = Color.green;

        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = transform.position + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = transform.position + new Vector3(0, y * cellSize, 0);
            Vector3 end = start + new Vector3(gridWidth * cellSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }

        if (grid != null)
        {
            Gizmos.color = Color.red;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y])
                    {
                        Vector3 center = transform.position + new Vector3(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2, 0);
                        Gizmos.DrawCube(center, new Vector3(cellSize * 0.8f, cellSize * 0.8f, 0.1f));
                    }
                }
            }
        }
    }
}
