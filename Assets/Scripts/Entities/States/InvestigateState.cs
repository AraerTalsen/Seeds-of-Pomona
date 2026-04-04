using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateState : BehaviorContext
{
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() 
    { 
        (new ObserveState(), 3),
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
            CurrentState = ChooseRandomState();
        }
    }

    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) => null;

    public override IBehaviorState GetCurrentState()
    {
        if(EntityProps.SuspiciousSpot != null)
        {
            EntityProps.TargetPos = EntityProps.SuspiciousSpot;
            CurrentState = PossibleStates[1].state;
        }

        return base.GetCurrentState();
    }
    
    public override void SelectNewState()
    {
        if(EntityProps.SuspiciousSpot != null)
        {
            EntityProps.TargetPos = EntityProps.SuspiciousSpot;
            CurrentState = PossibleStates[1].state;
        }
        else
        {
           base.SelectNewState(); 
        }
        //Debug.Log($"Investigate state will: {CurrentState}");
    }

    public override void Escape()
    {
        EntityProps.TargetPos = null;
        base.Escape();
    }
}
