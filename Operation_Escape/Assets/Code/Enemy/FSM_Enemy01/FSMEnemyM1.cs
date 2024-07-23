using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMEnemyM1 : StateMachine
{
    public float hp;
    public float Speed;
    public float dmg;
    public float visRang;
    public IAstarAI ai;
    public Transform target;
    public Rigidbody2D rb;
    [SerializeField]public Collider2D co;
    [HideInInspector]
    public WanderStateFSM wanderState;
    [HideInInspector]
    public N1AttackStateFSM N1Attack;
    [HideInInspector]
    public CheckDistanceStateFSM CheckDistance;
    [HideInInspector]
    public ChargeStateFSM Charge;
    public Dictionary<string, object> _dataContext = new Dictionary<string, object>();
    // Start is called before the first frame update
    private void Awake()
    {
        ai = gameObject.GetComponent<IAstarAI>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb=gameObject.GetComponent<Rigidbody2D>();
        SetData("ai", ai);
        SetData("target", target);
        SetData("co",co);
        wanderState = new WanderStateFSM(this);
        N1Attack = new N1AttackStateFSM(this);
        Charge = new ChargeStateFSM(this);
        CheckDistance = new CheckDistanceStateFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return wanderState;
    }

    public void SetData(string key, object value)
    {
        _dataContext[key] = value;
    }

    public override object Getdata(string key)
    {
        if (_dataContext.TryGetValue(key, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(transform.position, 10);
    }

}
