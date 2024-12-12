using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSMFSM : BaseState
{
    public DistanceSMFSM(FSMSMEnemySM stateEnemy) : base("CheckDistance", stateEnemy) { }
}
