using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnermy : MonoBehaviour
{
    [SerializeField] public List<StateMachine> enemy = new List<StateMachine>();
    [SerializeField] public List<IRestartOBJ> objInterac = new List<IRestartOBJ>();
    private int enemyCount;
    [SerializeField] public AutoDoor[] door;
    [SerializeField] public TriggerBoss areaBoss;
    public bool ready = true;
    public Transform checkPoint;
    public bool hasPlayer;
    public LayerMask enemyleLayer;
    public bool boss;
    private Collider2D colliderHit;

    // Start is called before the first frame update
    private void Awake()
    {
        foreach (var item in door)
        {
            item.area = this;
        }
        AddAllEnemy();
    }

    public void ResetMon()
    {
        if (ready)
        {
            if (!boss)
            {
                foreach (var item in enemy)
                {
                    item.gameObject.SetActive(true);
                    item.rb.velocity = Vector3.zero;
                    item.Health = item.maxHealth;
                    item.Reset();
                    enemyCount = enemy.Count;
                }

                foreach (var item in objInterac)
                {
                    item.Reset();
                }
            }
            else
            {
                foreach (var item in enemy)
                {
                    item.Health = item.maxHealth;
                }

                foreach (var item in objInterac)
                {
                    item.Reset();
                }
                areaBoss.Off();
            }

            hasPlayer = false;
        }       
    }

    public void AddAllEnemy()
    {
        Collider2D[] enemygameObject = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, enemyleLayer);
        foreach (var item in enemygameObject)
        {
            if (item.TryGetComponent(out StateMachine stateMachine))
            {
                enemy.Add(stateMachine);
                stateMachine.SetCombatPhase(GetComponent<AreaEnermy>());
            }
            else if (item.TryGetComponent(out IRestartOBJ restart))
            {
                objInterac.Add(restart);
            }
        }


        enemyCount = enemy.Count;
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

    public void Die()
    {
        enemyCount--;
        if (enemyCount == 0 && ready && !PlayerControl.control.isdaed)
        {
            foreach (var door in door)
            {
                door.Unlock();
            }
            PauseScene.spawnPoint = checkPoint;
            checkPoint.gameObject.GetComponentInChildren<SavePoint>().SetAc(true);
            ready = false;
        }
    }

    public void Lock()
    {
        foreach (var door in door)
        {
            door.LockOn();
        }
    }
}
