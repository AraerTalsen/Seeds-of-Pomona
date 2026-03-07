using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorContext : BehaviorContext
{
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() 
    { 
        (new InvestigateState(), 1),
        (new CombatState(), 0),
        (new NavigateState(), 0) 
    };

    public override float RecoveryTime { get; } = 8.0f;

    public EnemyBehaviorContext(EntityStateSupport entityStateSupport, EntityProperties entityProps)
    {
        EntityStateSupport = entityStateSupport;
        EntityProps = entityProps;

        CurrentState = ChooseRandomState();
        InitializeStates();
    }

    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) => null;

    public override void SelectNewState()
    {
        bool isTargetSpotted = EntityStateSupport.CheckForTargetEntities();
        
        if(!EntityProps.IsStunned)
        {
            if(!EntityProps.IsResting && !isTargetSpotted && !EntityProps.IsTargetLost)
            {
                CurrentState = ChooseRandomState();
            }
            else if(isTargetSpotted && EntityProps.DistFromTarget - EntityProps.MeleeRange <= 0.1f)
            {
                CurrentState = PossibleStates[1].state;
            }
            else if(isTargetSpotted || (!isTargetSpotted && EntityProps.IsTargetLost))
            {
                CurrentState = PossibleStates[2].state;
            }
            else if(EntityProps.IsResting)
            {
                CurrentState = null;
            }
        }
        else
        {
            CurrentState = null;
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        ReassessSystem();
        return base.GetCurrentState();
    }

    private void ReassessSystem()
    {
        if(EntityProps.IsStunned || EntityStateSupport.CheckForTargetEntities())
        {
            CurrentState = null;
        }
    }
}