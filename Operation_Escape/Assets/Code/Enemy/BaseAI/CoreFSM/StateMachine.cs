using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState curState;
    [Header("statusBasic")]
    public int maxHealth;
    public int Health;
    public int dmg;
    public float Speed;
    public float visRange;
    public bool attacking;
    public IAiAvoid ai;
    public LootTable lootDrop;
    public Transform target;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        ai = gameObject.GetComponent<IAiAvoid>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        ai.targetTransform = target;
        ai.maxspeed = Speed;

        curState = GetInitialState();
        if (curState != null)
        {
            curState.Enter();
        }        
    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (curState != null)
        {
            curState.UpdatePhysics();
        }
    }

    public void ChangState(BaseState newState)
    {
        curState.Exit();
        curState = newState;
        curState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    public virtual void CombatPhaseOn() { }
    public virtual void SetCombatPhase(AreaEnermy area) { }

}
