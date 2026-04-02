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

    public override void SelectNewState()
    {
        base.SelectNewState();
        Debug.Log("Pursuit is choosing a new state. Possible states are:");
        PossibleStates.ForEach(s => Debug.Log(s.state));
        Debug.Log($"Selected state is {CurrentState}");
    }

    public override void Escape()
    {
        Debug.Log("Escaping pursuit");
        base.Escape();
    }
}
