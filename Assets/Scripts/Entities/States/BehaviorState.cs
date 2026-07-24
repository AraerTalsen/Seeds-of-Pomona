using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class BranchConditionAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConditionTimestampAttribute : Attribute { }

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/States/Config Leaf State")]
public class BehaviorState : ScriptableObject, IBehaviorState, IRuntimeLauncher
{
    [SerializeField] private bool isSpecial = false;//Rename isSpecial and apply to all cooldown abilities. Check if any limitations with current implementation
    [SerializeField] private float recoveryTime;
    [SerializeField] protected EffectParameters parameters;
    public enum EffectLabel { stat, transform, instantiate, status }
    
    [SerializeField] private EffectLabel effectLabel;
    [SerializeReference] public Affecter affecter = new StatAffecter();
    [SerializeReference] public LifetimeRule rule;

    public BehaviorContext Context { get; set; }
    public virtual EntityProperties EntityProps { get; set; }
    public EntityStateSupport EntityStateSupport { get; set; }
    public virtual float RecoveryTime => recoveryTime;
    public bool IsCoolingDown { get; set; }
    [BranchCondition] public virtual bool IsValid
    {
        get
        {
            IsValidTimestamp = Time.time;
            return !IsCoolingDown;
        }
    }
    [ConditionTimestamp] protected float IsValidTimestamp { get; set; }
    public bool IsSpecial => isSpecial;
    public float NodeTimestamp { get; set; }

    public virtual async Task<IRuntimeEvent> LaunchEffect(EffectContext effectContext) 
    {
        IsCoolingDown = true;
        Context.Escape();
        NodeTimestamp = Time.time;
        IRuntimeEvent e = await affecter.CreateRuntimeEvent(effectContext);
        Recover();
        return e;
    }

    protected virtual void Recover()
    {
        if(!IsSpecial)
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
}
