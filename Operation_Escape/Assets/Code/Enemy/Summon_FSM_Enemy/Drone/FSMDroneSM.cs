using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMDroneSM : StateMachine , IDamageable
{
    public bool imortal { get; set; }

    [HideInInspector]
    CheckDistanceDFSM checkDistance;

    [HideInInspector]
    JAttackDFSM jAttack;

    private void Awake()
    {
        checkDistance = new CheckDistanceDFSM(this);
        jAttack = new JAttackDFSM(this);
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
