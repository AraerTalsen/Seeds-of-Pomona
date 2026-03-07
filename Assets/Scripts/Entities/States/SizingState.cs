using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizingState : BehaviorContext
{
    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new()
    {
        (new NavigateState(), 0)
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

    public override void SelectNewState()
    {
        if(!IsInPreferredRange())
        {
            CurrentState = ChooseRandomState();
        }
    }

    private bool IsInPreferredRange()
    {
        float dist = EntityProps.DistFromTarget;
        float min = EntityProps.PreferredRange.x;
        float max = EntityProps.PreferredRange.y;

        if(dist > min && dist < max)
        {
            Debug.Log("Target is out of range");
            Vector2 dirToTarget = (EntityProps.Transform.position - EntityProps.TargetTransform.position).normalized;
            float tollerantSpan = max - min;
            EntityProps.TargetPos = dirToTarget * (min + tollerantSpan / 2);
            CurrentState = PossibleStates[0].state;
            return false;
        }
            Debug.Log("Target is in range");

        return true;
    }

    protected override void ResetContextState()
    {
        CurrentState = null;
        ResetContextState();
    }
}
