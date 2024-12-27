using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart3EnemySM : FSMBaseBoss2EnemySM
{
    [Header("Cooldown")]
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    [HideInInspector]
    public H3IdleFSM Idle;
    [HideInInspector]
    public H3AttackFSM attack;
    [HideInInspector]
    public H3SummonFSM summon;

    private void Awake()
    {
        ResetPositions();
        Idle = new H3IdleFSM(this);
        attack = new H3AttackFSM(this);
        summon = new H3SummonFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return Idle;
    }

}
