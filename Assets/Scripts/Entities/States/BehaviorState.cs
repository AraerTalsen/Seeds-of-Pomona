using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class BranchConditionAttribute : Attribute { }

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/States/Config Leaf State")]
public class BehaviorState : ScriptableObject, IBehaviorState, IRuntimeLauncher
{
    [SerializeField] private bool isAttack = false;
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
    [BranchCondition] public virtual bool IsValid => !IsCoolingDown;
    public bool IsAttack => isAttack;

    public virtual Task<IRuntimeEvent> LaunchEffect(EffectContext effectContext)
    {
        Context.Escape();
        return affecter.CreateRuntimeEvent(effectContext, Callback);
    }

    protected void Callback(EffectContext context) => ResetContextState();

    protected virtual void ResetContextState()
    {
        if(!IsAttack)
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
