using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBaseBoss2EnemySM : StateMachine
{
    [Header("Spike")]
    [SerializeField] private GridBoss2 grid;
    public Spike spikeR;
    public Spike spikeZ;

    [Header("Minion")]
    public GameObject[] minion;
    public List<Transform> spawnPoint;
    private List<Transform> availablePositions = new List<Transform>();

    [Header("Shield")]
    public GuardShield Shield;

    public void AttackRSpike()
    {
        for (int i = 0; i < 10; i++)
        {
            grid.SpawnSpike(spikeR);
        }
    }

    public void AttackZSpike()
    {
        grid.SpawnSpike(spikeZ);
    }

    public void SummonMinion(int type)
    {
        int randomIndex = Random.Range(0, availablePositions.Count);
        Vector2 chosenPosition = availablePositions[randomIndex].position;

        availablePositions.RemoveAt(randomIndex);
        Instantiate(minion[type], chosenPosition, Quaternion.identity);
    }

    public void SummonMinion(int type,Vector2Int size)
    {
        grid.SpawnMinion(size, minion[type]);
    }

    public void ResetPositions()
    {
        availablePositions = new List<Transform>(spawnPoint);
    }
}
