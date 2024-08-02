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
            enemy.Add(state);
            state.setCombatPhase(GetComponent<AreaEnermy>());
        }
    }

    public void combatPhase()
    {
        foreach (var Enemy in enemy)
        {
            Debug.Log("chang");
            Enemy.combatPhaseOn();
        }
    }
}
