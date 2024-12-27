using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart4EnemySM : FSMBaseBoss2EnemySM
{
    [Header("Cooldown")]
    public float timeCooldownSpike;
    public float timeCooldownMinion;

    [HideInInspector]
    public H4IdleFSM Idle;
    [HideInInspector]
    public H4AttackFSM attack;
    [HideInInspector]
    public H4SummonFSM summon;

    private void Awake()
    {
        ResetPositions();
        Idle = new H4IdleFSM(this);
        attack = new H4AttackFSM(this);
        summon = new H4SummonFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return Idle;
    }

}
