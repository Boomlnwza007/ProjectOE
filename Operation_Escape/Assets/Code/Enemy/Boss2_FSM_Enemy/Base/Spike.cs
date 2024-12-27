using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public Vector2Int size;
    private Vector2Int gridPos;
    public int dmg;
    private GridBoss2 grid;

    public void Setup(GridBoss2 grid, Vector2Int gridPos)
    {
        this.grid = grid;
        this.gridPos = gridPos;
    }

    public void Remove()
    {
        grid.MarkGrid(gridPos,size,false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<IDamageable>().Takedamage(dmg,DamageType.Melee,0);
        }
    }
}
