using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMEnemySM : StateMachine, IDamageable
{
    [Header("status")]
    public bool cooldown;
    [HideInInspector]public float time;
    public float timeCooldown = 6f;
    public AreaEnermy areaEnermy;
    public bool imortal { get; set; }
    public string stateName;
    public EM_Animation animator;
    public LayerMask raycastMaskWay;
    public LayerMask raycastMask;
    public float jumpLength = 20;
    public float forcePush = 100;

    [Header("Dash")]
    public bool dash;
    public float dodgeMaxSpeed = 50f;
    public float dodgeMinimium = 30f;
    public float dodgeSpeedDropMultiplier = 5f;
    public float dodgeStopRange = 5f;
    [HideInInspector] public float rollSpeed;

    [HideInInspector]
    public WanderEMFSM wanderState;
    [HideInInspector]
    public NormalAttackEMFSM N1Attack;
    [HideInInspector]
    public CheckDistanceEMFSM CheckDistance;
    [HideInInspector]
    public ChargeEMFSM Charge;

    private void Awake()
    {
        wanderState = new WanderEMFSM(this);
        N1Attack = new NormalAttackEMFSM(this);
        CheckDistance = new CheckDistanceEMFSM(this);
        Charge = new ChargeEMFSM(this);
    }

    private void Update()
    {
        if (curState != null)
        {
            curState.UpdateLogic();
            stateName = curState.nameState;
        }

        if (cooldown)
        {
            time += Time.deltaTime;
            if (time > timeCooldown)
            {
                time = 0;
                cooldown = false;
            }
        }
    }

    protected override BaseState GetInitialState()
    {
        return wanderState;
    }

    public void CooldownApproching()
    {
        if (areaEnermy == null)
        {
            return;
        }

        float _cooldown = 5;
        List<StateMachine> enemy = areaEnermy.enemy;
        foreach (var item in enemy)
        {
            if (item.TryGetComponent<FSMMEnemySM>(out FSMMEnemySM fSMM))
            {
                if (fSMM != this)
                {
                    if (!fSMM.cooldown)
                    {
                        fSMM.cooldown = true;
                        fSMM.time = _cooldown;
                        _cooldown--;
                    }
                }
            }
        }
    }

    public void Takedamage(int damage, DamageType type, float knockBack)
    {
        Health -= damage;
        spriteFlash.Flash();
        switch (type)
        {
            case DamageType.Rang:
                //lootDrop.InstantiateLoot(3);
                break;
            case DamageType.Melee:
                lootDrop.InstantiateLoot(dropChange);
                break;
        }
        if (Health <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        if (areaEnermy != null)
        {
            areaEnermy.Die(this);
        }

        ChangState(wanderState);
        Destroy(gameObject);
    }

    public IEnumerator Imortal(float wait)
    {
        return null;
    }

    public override void CombatPhaseOn()
    {
        if (areaEnermy != null)
        {
            areaEnermy.AllcombatPhase();
        }
    }

    public override void SetCombatPhase(AreaEnermy area)
    {
        areaEnermy = area;
    }

    public async UniTask PreAttack(string name,float time)
    {
        float animationSpeed = 1 / time;
        animator.animator.speed = animationSpeed;
        animator.ChangeAnimationAttack(name);
        await UniTask.WaitForSeconds(time);
        animator.animator.speed = 1;
        animator.ChangeAnimationAttack("Normal");
    }

    public async UniTask PreAttackN(string name, float time)
    {
        animator.ChangeAnimationAttack(name);
        await UniTask.WaitForSeconds(time);
    }

    public async UniTask Attack(string name, float time)
    {
        animator.ChangeAnimationAttack(name);
        await UniTask.WaitForSeconds(time);
    }
    public async UniTask Attack(string name, int time)
    {
        animator.ChangeAnimationAttack(name);
        await UniTask.DelayFrame(time);
    }
    public async UniTask Attack(string name)
    {
        animator.animator.SetTrigger(name);
        await UniTask.WaitForSeconds(animator.TimePlayer());
    }

    public void Run(float multiply)
    {
        ai.maxspeed *= multiply;
        animator.animator.speed *= multiply;
    }

    public void Walk()
    {
        ai.maxspeed = Speed;
        animator.animator.speed = 1;
    }
}
