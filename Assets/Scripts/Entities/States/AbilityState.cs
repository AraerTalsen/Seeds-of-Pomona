using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityState : ScriptableObject, IBehaviorState, IAbilityEffect
{
    [SerializeField] protected PowerupEffect effect;
    public virtual EntityStateSupport EntityStateSupport { get; set; }

    public IBehaviorContext Context { get; set; }
    public EntityProperties EntityProps { get; set; }

    public float RecoveryTime { get; }

    public BehaviorContext HostContext { get; }
    public bool IsCoolingDown { get; set; }

    public bool IsValid { get; } = true;

    public abstract IEffectRuntime CreateEffectRuntime(EffectContext context);

    //public abstract void PerformAction();
}
