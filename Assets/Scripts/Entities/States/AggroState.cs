using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroState : BehaviorContext
{
    private float max;
    private float tolerance;

    public override bool IsAggro => true;

    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() 
    { 
        (new PursuitState(), 0),
        (new CombatState(), 1),
        (new SizingState(), 1)
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
            tolerance = EntityProps.PreferredTolerance;
            max = EntityProps.PreferredRange.y;
        }
    }
    
    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) => null;

    public override void SelectNewState()
    {
        if(EntityProps.DistFromTarget > max + tolerance)
        {
            CalculateTargetPos();
            CurrentState = PossibleStates[0].state;
        }
        else
        {
            CurrentState = ChooseRandomState();
        }
    }

    private void CalculateTargetPos()
    {
        float dist = EntityProps.DistFromTarget;
        Vector2 current = EntityProps.Transform.position;        
        Vector2 target = EntityProps.TargetTransform.position;
        Vector2 dirToTarget = (target - current).normalized;
        float magnitude = dist - max - tolerance + 0.1f;
        EntityProps.TargetPos = current + dirToTarget * magnitude;
    }

    public override IBehaviorState GetCurrentState()
    {
        if(EntityProps.DistFromTarget > max + tolerance)
        {
            CurrentState = null;
        }
        return base.GetCurrentState();
    }
}
