using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateState : BehaviorContext
{
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() 
    { 
        (new ObserveState(), 1),
        (new NavigateState(), 2) 
    };

    private EntityProperties entityProps;
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            InitializeStates();
            CurrentState = ChooseRandomState();
        }
    }

    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) => null;
}
