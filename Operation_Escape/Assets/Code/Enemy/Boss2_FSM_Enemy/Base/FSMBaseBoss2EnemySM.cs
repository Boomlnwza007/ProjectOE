using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMBaseBoss2EnemySM : StateMachine
{
    [Header("Spike")]
    [SerializeField] private GridBoss2 grid;
    public ID spike;

    [HideInInspector]public SpikeN spikeN;

    [Header("Minion")]
    public ID minion;
    public List<Transform> spawnPoint;
    private List<Transform> availablePositions = new List<Transform>();

    [Header("Shield")]
    public GuardShield shield;

    public void AttackRSpike()
    {
        Spike spikeR = spike.Item[0].GetComponent<Spike>();
        grid.SpawnAtPlayer(spikeR.gameObject,spikeR.size, 1);
        for (int i = 0; i < 9; i++)
        {
            grid.SpawnSpike(spikeR);
        }
    }

    public void AttackRSpike(float time)
    {
        Spike spikeR = spike.Item[0].GetComponent<Spike>();
        grid.SpawnAtPlayer(spikeR.gameObject, spikeR.size, 1);
        for (int i = 0; i < 9; i++)
        {
            grid.SpawnSpike(spikeR,time);
        }
    }

    public void AttackNSpike()
    {
        spikeN = Instantiate(spike.Item[2],ai.targetTransform.position,Quaternion.identity).GetComponent<SpikeN>();
    }

    public void AttackNSpike(float time)
    {
        Spike spikeR = spike.Item[0].GetComponent<Spike>();
        grid.SpawnAtPlayer(spikeR.gameObject,spikeR.size,time);
    }

    public void AttackZSpike()
    {
        SpikeZ.Setup();
        Instantiate(spike.Item[1].gameObject, ai.targetTransform.position, Quaternion.identity);
    }

    public void SummonMinion(int type)
    {
        int randomIndex = Random.Range(0, availablePositions.Count);
        Vector2 chosenPosition = availablePositions[randomIndex].position;

        availablePositions.RemoveAt(randomIndex);
        Instantiate(minion.Item[type], chosenPosition, Quaternion.identity);
    }

    public void SummonMinion(int type,Vector2Int size)
    {
        grid.SpawnMinion(size, minion.Item[type]);
    }

    public void ResetPositionsMInion()
    {
        availablePositions = new List<Transform>(spawnPoint);
    }

    public void ResetGrid()
    {
        grid.ResetGrid();
        grid.CheckGridObstacles();
    }
}
