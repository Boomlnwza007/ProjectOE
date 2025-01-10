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
    public Transform playerTransform;
    public int spawnRadius = 2;

    void Start()
    {
        grid = new bool[gridWidth, gridHeight];
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        CheckGridObstacles();
    }

    public void SpawnSpike(Spike spikeType)
    {
        Vector2Int? spawnPosition = FindAvailablePosition(spikeType.size);

        if (spawnPosition != null)
        {
            Vector2Int pos = spawnPosition.Value;
            Vector3 worldPos = GridToWorldPosition(pos);

            Spike spike = Instantiate(spikeType.gameObject, worldPos, Quaternion.identity, spawnParent).GetComponent<Spike>();
            spike.Setup(this, pos);

            MarkGrid(pos, spikeType.size, true);
        }
        else
        {
            Debug.Log("No available space for this tower!");
        }
    }

    public void SpawnSpike(Spike spikeType,float time)
    {
        Vector2Int? spawnPosition = FindAvailablePosition(spikeType.size);

        if (spawnPosition != null)
        {
            Vector2Int pos = spawnPosition.Value;
            Vector3 worldPos = GridToWorldPosition(pos);

            Spike spike = Instantiate(spikeType.gameObject, worldPos, Quaternion.identity, spawnParent).GetComponent<Spike>();
            spike.Setup(this, pos);
            spike.Time(time);
            MarkGrid(pos, spikeType.size, true);
        }
        else
        {
            Debug.Log("No available space for this tower!");
        }
    }

    public void SpawnMinion(Vector2Int size ,GameObject minionType)
    {
        Vector2Int? spawnPosition = FindAvailablePosition(size);

        if (spawnPosition != null)
        {
            Vector2Int pos = spawnPosition.Value;
            Vector3 worldPos = GridToWorldPosition(pos);

            FSMMinion2EnemySM minion = Instantiate(minionType.gameObject, worldPos, Quaternion.identity).GetComponent<FSMMinion2EnemySM>();
            minion.Setup(this,pos);
            MarkGrid(pos, size, true);
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
                if (CanPlaceFromAnyCorner(new Vector2Int(x, y), size))
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

    private bool CanPlace(Vector2Int start, Vector2Int size)
    {
        if (start.x < 0 || start.y < 0 || start.x + size.x > gridWidth || start.y + size.y > gridHeight)
        {
            return false;
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (grid[start.x + x, start.y + y]) return false;
            }
        }

        return true;
    }

    private bool CanPlaceFromAnyCorner(Vector2Int center, Vector2Int size)
    {
        Vector2Int[] corners = new Vector2Int[]
        {
        new Vector2Int(center.x, center.y),                             // มุมบนซ้าย
        new Vector2Int(center.x - size.x + 1, center.y),               // มุมบนขวา
        new Vector2Int(center.x, center.y - size.y + 1),               // มุมล่างซ้าย
        new Vector2Int(center.x - size.x + 1, center.y - size.y + 1)   // มุมล่างขวา
        };

        foreach (var corner in corners)
        {
            if (CanPlace(corner, size))
            {
                return true;
            }
        }

        return false;
    }

    public void MarkGrid(Vector2Int start, Vector2Int size, bool isOccupied)
    {
        if (start.x < 0 || start.y < 0 || start.x + size.x > gridWidth || start.y + size.y > gridHeight)
        {
            return;
        }

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

    public void CheckGridObstacles()
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

    public void SpawnAroundPlayer(Spike spikeType)
    {
        Vector2Int playerGridPos = FindPlayer();
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        for (int x = -spawnRadius; x <= spawnRadius; x++)
        {
            for (int y = -spawnRadius; y <= spawnRadius; y++)
            {
                Vector2Int checkPos = playerGridPos + new Vector2Int(x, y);

                if (IsValidGridPosition(checkPos) && CanPlaceFromAnyCorner(checkPos, spikeType.size))
                {
                    possiblePositions.Add(checkPos);
                }
            }
        }

        int randomIndex = Random.Range(0, possiblePositions.Count);
        Vector2Int spawnPos = possiblePositions[randomIndex];
        possiblePositions.RemoveAt(randomIndex);

        Vector3 worldPos = GridToWorldPosition(spawnPos);
        Spike spike = Instantiate(spikeType.gameObject, worldPos, Quaternion.identity).GetComponent<Spike>();
        spike.Setup(this, playerGridPos);
        MarkGrid(spawnPos, spikeType.size, true);
    }

    public void SpawnAtPlayer(Spike spikeType, float time)
    {
        Vector2Int playerGridPos = FindPlayer();
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        if (CanPlaceFromAnyCorner(playerGridPos, spikeType.size))
        {
            //possiblePositions.Add(playerGridPos);
            Vector3 worldPos = GridToWorldPosition(playerGridPos);
            Spike spike = Instantiate(spikeType.gameObject, worldPos, Quaternion.identity).GetComponent<Spike>();
            spike.Time(time);
            spike.Setup(this, playerGridPos);
            MarkGrid(playerGridPos, spikeType.size, true);
        }
        else
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    Vector2Int checkPos = playerGridPos + new Vector2Int(x, y);

                    if (IsValidGridPosition(checkPos) && CanPlaceFromAnyCorner(checkPos, spikeType.size))
                    {
                        possiblePositions.Add(checkPos);
                    }
                }
            }

            Vector2Int spawnPos = possiblePositions[Random.Range(0, possiblePositions.Count)];

            Vector3 worldPos = GridToWorldPosition(spawnPos);
            Spike spike = Instantiate(spikeType.gameObject, worldPos, Quaternion.identity).GetComponent<Spike>();
            spike.Time(time);
            spike.Setup(this, playerGridPos);
            MarkGrid(spawnPos, spikeType.size, true);
        }      


    }

    private bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    private Vector2Int FindPlayer()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = GridToWorldPosition(new Vector2Int(x, y));
                Collider2D collider = Physics2D.OverlapBox(worldPos, new Vector2(cellSize, cellSize), 0, LayerMask.GetMask("Player"));
                if (collider != null)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(0, 0);
    }

    public void ResetGrid()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = false;
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
