using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityState : ScriptableObject, IBehaviorState
{
    public virtual EntityStateSupport EntityStateSupport { get; set; }

    public IBehaviorContext Context { get; set; }
    public EntityProperties EntityProps { get; set; }

    public float RecoveryTime { get; }


    public abstract void PerformAction();
}
