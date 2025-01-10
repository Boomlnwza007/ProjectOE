using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public Vector2Int size;
    public Animator animator;
    private Vector2Int gridPos;
    private GridBoss2 grid;

    public void Setup(GridBoss2 grid, Vector2Int gridPos)
    {
        this.grid = grid;
        this.gridPos = gridPos;
    }

    public void Time(float time)
    {
        float newSpeed = 1 / time; 
        animator.speed = newSpeed;
    }

    public void Remove()
    {
        grid.MarkGrid(gridPos,size,false);
        Destroy(gameObject);
    }

    public void normal()
    {
        animator.speed = 1;
    }
}
