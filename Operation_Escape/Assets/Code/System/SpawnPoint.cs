using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnPoint
{
    public int id;
    public Vector3 spawnPosition;
    public SpawnPoint(int id, Vector3 spawnPosition)
    {
        this.id = id;
        this.spawnPosition = spawnPosition;
    }
}
