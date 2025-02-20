using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Mark : MonoBehaviour
{
    [Header("Strike")]
    public Transform[] top;
    public Transform[] down;
    public Transform[] left;
    public Transform[] right;

    [Header("Minion")]
    public List<Transform> monSpawn;

    [Header("Grid")]
    public GridBoss2 grid;

    [HideInInspector] public FSMBoss2EnemySM state;
}
