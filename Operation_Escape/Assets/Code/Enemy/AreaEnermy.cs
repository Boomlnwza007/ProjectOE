using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnermy : MonoBehaviour
{
    [SerializeField] public List<StateMachine> enemy = new List<StateMachine>();
    [SerializeField] public AutoDoor[] door;
    [SerializeField] public TriggerBoss areaBoss;
    public bool ready = true;
    public Transform checkPoint;
    public bool hasPlayer;
    public LayerMask enemyleLayer;
    public bool boss;

    // Start is called before the first frame update
    private void Awake()
    {
        foreach (var item in door)
        {
            item.area = this;
        }
        AddAllEnemy();
    }

    private void Update()
    {
        if (enemy.Count == 0 && ready)
        {
            foreach (var door in door)
            {
                door.Unlock();                
            }
            PauseScene.spawnPoint = checkPoint;            
            ready = false;
        }
    }

    public void ResetMon()
    {
        if (!boss)
        {
            foreach (var item in enemy)
            {
                item.Health = item.maxHealth;
            }
        }
        else
        {
            foreach (var item in enemy)
            {
                item.Health = item.maxHealth;
            }
            areaBoss.Off();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasPlayer = true;
            PauseScene.area = this;
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

    public void Lock()
    {
        foreach (var door in door)
        {
            door.LockOn();
        }
    }
}
