using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControll : MonoBehaviour
{
    [SerializeField] private StateMachine mon;
    [SerializeField] private Animator animator;
    [SerializeField] private FollowAttackType followAttack;
    public bool meleeFollow = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        followAttack.GetData(animator, mon);
    }

    private void Update()
    {
        if (meleeFollow)
        {
            followAttack.DiractionAttack();
        }
    }

    public void AttackFollowTrue()
    {
        meleeFollow = true;
    }

    public void AttackFollowFalse()
    {
        meleeFollow = false;
    }
    public void AttackRange(float Range)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Range);
        foreach (var hit in colliders)
        {
            IDamageable player = hit.GetComponent<IDamageable>();
            if (hit.CompareTag("Player"))
            {
                player.Takedamage(mon.dmg, DamageType.Melee, 0);
            }
        }
    }

}
