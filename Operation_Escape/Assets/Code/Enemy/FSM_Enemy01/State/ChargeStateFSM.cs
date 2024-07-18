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
    Rigidbody2D rb;
    Transform bulletPos;
    float time;
    bool canApproching;
    bool wait;
    bool follow;
    bool canDodge;
    int maxDodge;
    int dodge;
    public ChargeStateFSM(FSMEnemyM1 stateMachine) : base("Charge", stateMachine) { }
    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
        canDodge = true;
        wait = false;
        time = 0;
        dodge = 0;
        follow = true;
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
            maxDodge = Random.Range(2, 5);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();        
        float distance = Vector2.Distance(ai.position, target.position);
        if (follow)
        {
            ai.destination = target.position;
            //Vector3 rotation = ai.position - target.position;
            //float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            //ai.rotation = Quaternion.Euler(0, 0, rot + 90);
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
            if (distance <= 2)
            {
                stateMachine.ChangState(((FSMEnemyM1)stateMachine).N1Attack);
            }
            time += Time.deltaTime;
            if (time > 2f && canDodge)
            {
                ai.maxSpeed = speed;
                time = 0;
                wait = false;                
            }

            if (dodge >= maxDodge)
            {
                canDodge = false;
                if (time > 3f)
                {
                    stateMachine.ChangState(((FSMEnemyM1)stateMachine).CheckDistance);
                }
            }

            Debug.Log(canDodge + " = " +dodge+" >= " + maxDodge );
            if (canDodge&&!wait)
            {
                if (DeBullet())
                {
                    Debug.Log("can dodge");
                    DodgeBullet();
                }
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

    public void DodgeBullet()
    {
        var normal = (ai.position - bulletPos.position).normalized;
        var tangent = Vector3.Cross(normal, new Vector3(0, 0, 1)); 
        wait = true;
        //ai.maxSpeed = speed*15;
        //ai.destination = ai.position + tangent * 10;
        ai.Teleport(ai.position + tangent * 5f);
        dodge++;
        
        Debug.Log("dodge");
    }

    public void GetData()
    {
        ai = (IAstarAI)stateMachine.Getdata("ai");
        target = (Transform)stateMachine.Getdata("target");
        speed = ((FSMEnemyM1)stateMachine).Speed;
        co = ((FSMEnemyM1)stateMachine).co;
        rb = ((FSMEnemyM1)stateMachine).rb;
    }

}
