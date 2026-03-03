using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorState : IBehaviorState
{
    public IBehaviorContext Context { get; set; }
    public virtual EntityProperties EntityProps { get; set; }
    public EntityStateSupport EntityStateSupport { get; set; }
    public virtual float RecoveryTime { get; }

    public abstract IEffectRuntime CreateEffectRuntime(EffectContext effectContext);

    //public abstract void PerformAction();

    protected void ResetContextState()
    {
        EntityProps.TargetPos = null;
        Context.CurrentState = null;
        if (!EntityProps.IsTracking)
        {
            EntityProps.Recover(RecoveryTime);
        }
    }
}
