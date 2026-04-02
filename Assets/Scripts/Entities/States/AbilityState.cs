using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityState : ScriptableObject, IBehaviorState, IAbilityEffect
{
    private enum ContextType
    {
        Base,
        Investigate,
        Aggro,
        Pursuit,
        Sizing,
        Combat
    }

    private Dictionary<ContextType, System.Type> ContextEnumToType = new()
    {
        {ContextType.Base, typeof(EnemyBehaviorContext)},
        {ContextType.Investigate, typeof(InvestigateState)},
        {ContextType.Aggro, typeof(AggroState)},
        {ContextType.Pursuit, typeof(PursuitState)},
        {ContextType.Sizing, typeof(SizingState)},
        {ContextType.Combat, typeof(CombatState)},
    };
    
    [SerializeField] protected PowerupEffect effect;
    [SerializeField] private ContextType contextType;
    public virtual EntityStateSupport EntityStateSupport { get; set; }

    public BehaviorContext Context { get; set; }
    public EntityProperties EntityProps { get; set; }

    public virtual float RecoveryTime { get; }

    public System.Type HostContext => ContextEnumToType[contextType];
    public bool IsCoolingDown { get; set; }

    public virtual bool IsValid { get; } = true;

    public abstract IEffectRuntime CreateEffectRuntime(EffectContext context);

    protected virtual void ResetContextState()
    {
        Context.Escape();
        
        if(!Context.IsAggro)
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
