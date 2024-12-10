using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSMEnemySM : StateMachine ,IDamageable
{
    public bool imortal { get; set; }

    [HideInInspector]
    public CheckDistanceSMFSM checkDistance;
    [HideInInspector]
    public SummonSMFSM summon;

    private void Awake()
    {
        checkDistance = new CheckDistanceSMFSM(this);
        summon = new SummonSMFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return base.GetInitialState();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Imortal(float wait)
    {
        throw new System.NotImplementedException();
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        throw new System.NotImplementedException();
    }
}
