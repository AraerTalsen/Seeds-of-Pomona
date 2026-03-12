using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorState : IBehaviorState
{
    public IBehaviorContext Context { get; set; }
    public virtual EntityProperties EntityProps { get; set; }
    public EntityStateSupport EntityStateSupport { get; set; }
    public virtual float RecoveryTime { get; }
    protected virtual string LoadEffect { get; }
    protected PowerupEffect powerupEffect;
    public bool IsCoolingDown { get; set; }
    public virtual bool IsValid { get; } = true;

    public abstract IEffectRuntime CreateEffectRuntime(EffectContext effectContext);

    //public abstract void PerformAction();

    protected virtual void ResetContextState()
    {
        Context.Escape();
        
        if(powerupEffect == null)
        {
            PeacefulRecover();
        }
        else
        {
            CombatRecover();
        }
    }

    private void PeacefulRecover()
    {
        if (!EntityProps.IsTracking)
        {
            EntityProps.Recover(RecoveryTime);
        }
    }

    private void CombatRecover()
    {
        EntityProps.CombatRecover(this, RecoveryTime);
    }

    public void TryLoadPowerupEffect()
    {
        if(!string.IsNullOrEmpty(LoadEffect))
        {
            powerupEffect = Object.Instantiate(Resources.Load<PowerupEffect>(LoadEffect));
        }
    } 
}
