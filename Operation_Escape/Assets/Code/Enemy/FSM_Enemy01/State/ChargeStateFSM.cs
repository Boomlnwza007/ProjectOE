using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeStateFSM : BaseState
{
    public IAstarAI ai;
    public Transform target;
    public float speed;
    public Collider2D co;
    Transform bulletPos;
    float time;
    bool canApproching;
    bool wait;
    bool timeOn;
    public ChargeStateFSM(FSMEnemyM1 stateMachine) : base("Charge", stateMachine) { }
    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
        dod = true;
        wait = false;
        time = 0;
        timeOn = false;
        GetData();
        ai.destination = target.position;        
        if (Random.Range(0, 100) > 50)
        {
            canApproching = true;
            ai.maxSpeed = speed * 2.5f;
        }
        else
        {
            canApproching = false;
            ai.maxSpeed = speed * 1.5f;
            if (Random.Range(80, 100) > 50)
            {
                dod = true;
            }
        }
        canApproching = false;
        ai.maxSpeed = speed * 1.5f;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        Vector3 rotation = ai.position - target.position;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        ai.rotation = Quaternion.Euler(0, 0, rot + 90);
        float distance = Vector2.Distance(ai.position, target.position);
        ai.destination = target.position;
        if (timeOn)
        {
            time += Time.deltaTime;
        }
        if (canApproching)
        {
            time += Time.deltaTime;
            if (distance<=2 || time > 1f)
            {
                ai.maxSpeed = speed;
                if (!wait)
                {
                    Collider2D[] Collider = Physics2D.OverlapCircleAll(ai.position, 2);
                    foreach (var item in Collider)
                    {
                        if (item.tag == "Player")
                        {
                            stateMachine.ChangState(((FSMEnemyM1)stateMachine).N1Attack);
                            return;
                        }
                        else
                        {
                            wait = true;                            
                        }
                    }
                    if (wait)
                    {
                        time = 0;
                    }
                }
                else
                {
                    ai.canMove = false;
                    if (time > 2)
                    {
                        ai.canMove = true;
                        stateMachine.ChangState(((FSMEnemyM1)stateMachine).CheckDistance);
                        return;
                    }
                }
               
            }
        }
        else
        {
            Debug.Log("b " + ai.canMove);
            if (distance <= 5 && !wait)
            {
                timeOn = true;
                ai.canMove = false;                     
            }
            if (time >= 2 && !wait)
            {
                wait = true;
                ai.canMove = true;
                stateMachine.ChangState(((FSMEnemyM1)stateMachine).CheckDistance);
            }

            if (dod)
            {
                if (DeBullet())
                {
                    DodgeBullet();
                }
                dod = false;
            }
        }
    }

    public bool DeBullet()
    {
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCollider(co, filter, results);
        foreach (var item in results)
        {
            if (item.tag == "Bullet")
            {
                bulletPos = item.transform;
                return true;
            }
        }
        Debug.Log(results);
        results.Clear();
        return false;
    }

    bool dod = true;
    public void DodgeBullet()
    {
        var normal = (ai.position - bulletPos.position).normalized;
        var tangent = Vector3.Cross(normal, new Vector3(0, 0, 1));
        ai.Teleport(target.position + normal * 2 + tangent * 5);
    }

    public void GetData()
    {
        ai = (IAstarAI)stateMachine.Getdata("ai");
        target = (Transform)stateMachine.Getdata("target");
        speed = ((FSMEnemyM1)stateMachine).Speed;
        co = ((FSMEnemyM1)stateMachine).co;
    }

}
