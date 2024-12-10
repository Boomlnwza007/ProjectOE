using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JAttackDFSM : BaseState
{

    public JAttackDFSM(FSMDroneSM stateEnemy) : base("jumpAttack", stateEnemy) { }

}
