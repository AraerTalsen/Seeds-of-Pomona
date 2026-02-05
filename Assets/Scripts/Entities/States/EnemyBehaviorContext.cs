using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorContext : BehaviorContext
{
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() { (new InvestigateState(), 50), (new NavigateState(), 0) };
    public override float RecoveryTime { get; } = 8.0f;

    public EnemyBehaviorContext(EntityStateSupport entityStateSupport, EntityProperties entityProps)
    {
        EntityStateSupport = entityStateSupport;
        EntityProps = entityProps;

        CurrentState = PossibleStates[0].state;
        InitializeStates();
    }

    public override void PerformAction()
    {
        SearchForTargetEntity();
        if (!EntityProps.IsResting)
        {
            currentState.PerformAction();
        }

    }

    private void SearchForTargetEntity()
    {
        bool targetFound = EntityStateSupport.CheckForTargetEntities();
        if (targetFound && CurrentState != PossibleStates[1].state)
        {
            CurrentState = PossibleStates[1].state;
        }
        else if (!targetFound && !EntityProps.IsTargetLost)
        {
            CurrentState = null;
        }
    }
}