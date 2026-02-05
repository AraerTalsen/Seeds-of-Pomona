using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateState : BehaviorContext
{
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() { (new ObserveState(), 33), (new NavigateState(), 55) };

    private EntityProperties entityProps;
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            InitializeStates();
            CurrentState = null;
        }
    }

    public override void PerformAction()
    {
        CurrentState.PerformAction();
    }
}
