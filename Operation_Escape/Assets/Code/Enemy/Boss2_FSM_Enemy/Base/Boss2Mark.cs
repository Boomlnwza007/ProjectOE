using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Mark : MonoBehaviour
{
    public static Boss2Mark mark;
    [Header("Strike")]
    public Transform[] top;
    public Transform[] down;
    public Transform[] left;
    public Transform[] right;

    [Header("Minion")]
    public List<Transform> monSpawn;

    [Header("Grid")]
    public GridBoss2 grid;

    [Header("Jump")]
    public Transform jumpCenter;
    public Transform startLaser;

    [Header("SpawnObj")]
    public Transform spark;
    public GameObject endanimation;

    [HideInInspector] public FSMBoss2EnemySM state;
}
