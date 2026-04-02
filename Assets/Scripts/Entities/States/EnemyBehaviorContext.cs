using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorContext : BehaviorContext
{
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() 
    { 
        (new InvestigateState(), 1),
        (new AggroState(), 0)
    };

    public override float RecoveryTime { get; } = 8.0f;
    public override Dictionary<System.Type, IBehaviorContext> ContextRegistry { get; set; } = new();

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
            if(!EntityProps.IsResting && !isTargetSpotted && EntityProps.MemorizedTargetPos == null)
            {
                CurrentState = ChooseRandomState();
            }
            else if(isTargetSpotted || EntityProps.MemorizedTargetPos != null)
            {
                CurrentState = PossibleStates[1].state;
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
        EntityProps.NavMeshAgent.isStopped = true;
        //EntityProps.Rigidbody.velocity = Vector2.zero;
        ReassessSystem();
        return base.GetCurrentState();
    }

    private void ReassessSystem()
    {
        bool isTargetSpotted = EntityStateSupport.CheckForTargetEntities();

        if(EntityProps.IsStunned || isTargetSpotted)
        {
            CurrentState = null;
        }

        if(isTargetSpotted)
        {
            EntityProps.SuspiciousSpot = null;
        }
    }
}