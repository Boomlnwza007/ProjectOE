using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnermy : MonoBehaviour
{
    [SerializeField] List<StateMachine> enemy = new List<StateMachine>();
    public LayerMask enemyleLayer;
    // Start is called before the first frame update

    private void Start()
    {
        Collider2D[] enemygameObject = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0, enemyleLayer);
        foreach (var item in enemygameObject)
        {
            StateMachine state = item.GetComponent<StateMachine>();
            if (state != null)
            {
                enemy.Add(state);
                state.SetCombatPhase(GetComponent<AreaEnermy>());
            }            
        }
    }

    public void combatPhase()
    {
        foreach (var Enemy in enemy)
        {
            Debug.Log("chang");
            Enemy.CombatPhaseOn();
        }
    }
}
