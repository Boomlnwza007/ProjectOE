using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart2EnemySM : FSMBaseBoss2EnemySM
{
    [Header("Cooldown")]
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    [HideInInspector]
    public H2IdleFSM Idle;
    [HideInInspector]
    public H2AttackFSM attack;
    [HideInInspector]
    public H2SummonFSM summon;

    private void Awake()
    {
        ResetPositions();
        Idle = new H2IdleFSM(this);
        attack = new H2AttackFSM(this);
        summon = new H2SummonFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return Idle;
    }

}
