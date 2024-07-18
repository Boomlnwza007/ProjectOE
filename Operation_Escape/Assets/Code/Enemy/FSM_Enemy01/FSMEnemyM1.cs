using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMEnemyM1 : StateMachine
{
    [Header("Line Of sight") ]
    public float lineSight;
    public float angleSight;
    public float rangSight;

    public float Speed;
    IAstarAI ai;
    Transform target;
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
