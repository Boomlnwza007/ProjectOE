using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnermy : MonoBehaviour
{
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    [SerializeField] public ID idMonNormal;
    [SerializeField] public ID idMonBoss;
    [SerializeField] public List<StateMachine> enemy = new List<StateMachine>();
    [SerializeField] public List<IRestartOBJ> objInterac = new List<IRestartOBJ>();
    private int enemyCount;
    [SerializeField] public AutoDoor[] door;
    [SerializeField] public TriggerBoss areaBoss;
    public DelBulletAll delBulletAll;
    public bool ready = true;
    public bool hasPlayer;
    public LayerMask enemyleLayer;
    public bool boss;
    public static HashSet<AreaEnermy> area = new HashSet<AreaEnermy>();

    // Start is called before the first frame update
    private void Awake()
    {
        foreach (var item in door)
        {
            item.area = this;
        }
        AddAllEnemy();
        if (boss)
        {
            foreach (var enemySpawn in enemy)
            {
                enemySpawn.attacking = false;
                enemySpawn.SetCombatPhase(this);
                areaBoss?.SetUp(enemySpawn, enemySpawn.GetComponentInChildren<UIBoss>());
            }
        }
    }

    public void ResetMon()
    {
        if (ready)
        {
            if (!boss)
            {               
                ClaerMon();

                foreach (var enemySpawn in spawnPoints)
                {
                    StateMachine mon = Instantiate(idMonNormal.Item[enemySpawn.id], enemySpawn.spawnPosition, Quaternion.identity).GetComponent<StateMachine>();
                    mon.attacking = false;
                    mon.SetCombatPhase(this);
                    enemy.Add(mon);
                }  
                enemyCount = enemy.Count;

                //ReItem();

                delBulletAll.DestroyBullet();
                delBulletAll.DestroyBody();

            }
            else
            {
                ClaerMon();
                foreach (var enemySpawn in spawnPoints)
                {
                    StateMachine mon = Instantiate(idMonBoss.Item[enemySpawn.id], enemySpawn.spawnPosition, Quaternion.identity).GetComponent<StateMachine>();
                    mon.attacking = false;
                    mon.SetCombatPhase(this);
                    enemy.Add(mon);
                    areaBoss.SetUp(mon,mon.GetComponentInChildren<UIBoss>());
                }
                enemyCount = enemy.Count;
                //ReItem();
                delBulletAll.DestroyBullet();
                delBulletAll.DestroyBody();
            }

            hasPlayer = false;
        }       
    }

    public void ClaerMon()
    {
        ClaerMonMinion();
        foreach (var e in enemy)
        {
            e.ClearObj();
            Destroy(e.gameObject);
        }
        enemy.Clear();
        
    }

    public void ClaerMonMinion()
    {
        Collider2D[] enemygameObject = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, enemyleLayer);
        foreach (var item in enemygameObject)
        {
            if (item.TryGetComponent<StateMachine>(out var stateMachine) && stateMachine.ID == 0)
            {
                if (item.TryGetComponent<IDamageable>(out var damage))
                {
                    damage.Die();
                }
            }
        }

        for (int i = EggBoss2.eggSpawn.Count - 1; i >= 0; i--)
        {
            EggBoss2.eggSpawn[i].Die();
        }
        EggBoss2.eggSpawn.Clear();
    }

    public void ReItem()
    {
        foreach (var item in objInterac)
        {
            item.Reset();
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
                spawnPoints.Add(new SpawnPoint(stateMachine.ID, stateMachine.gameObject.transform.position));
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
            if (ready)
            {
                hasPlayer = true;
                area.Add(this);
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

    public void Die(StateMachine state)
    {
        enemyCount--;
        enemy.Remove(state);
        if (enemyCount == 0 && ready && !PlayerControl.control.isdaed)
        {
            foreach (var door in door)
            {
                door.Unlock();
            }
        }
    }

    public void Lock()
    {
        foreach (var door in door)
        {
            door.LockOn();
        }
    }

    public void ForceLock()    
    {
        foreach (var door in door)
        {
            door.ForceLock();
        }
    }
}
