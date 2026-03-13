using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState : BehaviorContext
{
    public override bool IsAggro => true;
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() 
    { 
        (new NavigateState(), 1)
    };

    private EntityProperties entityProps;
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            ContextRegistry = Context.ContextRegistry;
            AddToRegistry(this);
            InitializeStates();
            CurrentState = PossibleStates[0].state;
        }
    }

    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) => null;
}
