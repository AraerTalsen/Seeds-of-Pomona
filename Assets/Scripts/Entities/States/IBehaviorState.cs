using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviorState : IAbilityEffect
{
    public BehaviorContext Context { get; set; }
    public EntityProperties EntityProps { get; set; }
    public EntityStateSupport EntityStateSupport { get; set; }
    public float RecoveryTime { get; }
    public bool IsCoolingDown { get; set; }
    public bool IsValid { get; }
}
