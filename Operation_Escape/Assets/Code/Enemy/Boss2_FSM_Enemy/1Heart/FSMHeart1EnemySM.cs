using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHeart1EnemySM : FSMBaseBoss2EnemySM
{
    [Header("Cooldown")]
    public float timeCooldownSpike;

    [HideInInspector]
    public H1IdleFSM Idle;
    [HideInInspector]
    public H1AttackFSM attack;

    private void Awake()
    {
        ResetPositions();
        Idle = new H1IdleFSM(this);
        attack = new H1AttackFSM(this);
    }

    protected override BaseState GetInitialState()
    {
        return Idle;
    }

}
