using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorContext : BehaviorContext
{
    public override List<(IBehaviorState state, int maxThreshold)> PossibleStates { get; } = new() { (new InvestigateState(), 0), (new NavigateState(), 0) };
    private IBehaviorState currentState;
    public override IBehaviorState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value ?? PossibleStates[0].state;
        }
    }
    public override float RecoveryTime { get; } = 8.0f;

    private readonly EntityStateSupport entityStateSupport;

    public EnemyBehaviorContext(EntityStateSupport entityStateSupport, EntityProperties entityProps)
    {
        this.entityStateSupport = entityStateSupport;
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
        bool targetFound = entityStateSupport.CheckForTargetEntities();
        if (targetFound && CurrentState == PossibleStates[0].state)
        {
            CurrentState = PossibleStates[1].state;
        }
        else if (!targetFound && !EntityProps.IsTargetLost && CurrentState == PossibleStates[1].state)
        {
            CurrentState = PossibleStates[0].state;
        }
    }
}