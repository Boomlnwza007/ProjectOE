using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnermy : MonoBehaviour
{
    [SerializeField] public List<StateMachine> enemy = new List<StateMachine>();
    [SerializeField] public AutoDoor[] door;

    public LayerMask enemyleLayer;
    // Start is called before the first frame update

    private void Start()
    {
        AddAllEnemy();
    }

    private void Update()
    {
        if (enemy.Count == 0)
        {
            foreach (var door in door)
            {
                door.Unlock();
            }
        }
    }

    public void AddAllEnemy()
    {
        Collider2D[] enemygameObject = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, enemyleLayer);
        foreach (var item in enemygameObject)
        {
            StateMachine state = item.GetComponent<StateMachine>();
            if (state != null)
            {
                enemy.Add(state);
                state.SetCombatPhase(GetComponent<AreaEnermy>());
            }
        }
    }

    public void AllcombatPhase()
    {
        foreach (var Enemy in enemy)
        {
            Enemy.attacking = true;
        }
    }

    public float Size()
    {
        float size = transform.localScale.x < transform.localScale.y ? transform.localScale.x : transform.localScale.y;
        return size;
    }

    public void Die(StateMachine _enemy)
    {
        enemy.Remove(_enemy);
    }
}
