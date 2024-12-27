using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart5EnemySM : FSMBaseBoss2EnemySM
{
    [Header("Cooldown")]
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    public BaseAnimEnemy animator;

    [HideInInspector]
    public H5IdleFSM Idle;
    [HideInInspector]
    public H5AttackFSM attack;
    [HideInInspector]
    public H5SummonFSM summon;

    private void Awake()
    {
        ResetPositions();
        Idle = new H5IdleFSM(this);
        attack = new H5AttackFSM(this);
        summon = new H5SummonFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return Idle;
    }

}
