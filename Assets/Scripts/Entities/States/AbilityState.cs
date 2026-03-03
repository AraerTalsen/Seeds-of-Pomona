using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityState<TContext> : ScriptableObject, IBehaviorState, IAbilityEffect
{
    [SerializeField] protected PowerupEffect effect;
    public virtual EntityStateSupport EntityStateSupport { get; set; }

    public IBehaviorContext Context { get; set; }
    public EntityProperties EntityProps { get; set; }

    public float RecoveryTime { get; }

    public TContext HostContext { get; }

    public abstract IEffectRuntime CreateEffectRuntime(EffectContext context);

    //public abstract void PerformAction();
}
