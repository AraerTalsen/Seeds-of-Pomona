using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizingState : BehaviorContext
{
    private bool isRetreating = false;
    private float min;
    private float max;

    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new()
    {
        (new NavigateState(), 0),
        (new ObserveState(), 1)
    };

    private EntityProperties entityProps;
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            InitializeStates();
            CurrentState = PossibleStates[0].state;
            min = EntityProps.PreferredRange.x;
            max = EntityProps.PreferredRange.y;
        }
    }

    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) => null;

    public override void SelectNewState()
    {
        if(IsInPreferredRange())
        {
            CurrentState = ChooseRandomState();
        }
        Debug.Log($"SizingState CurrentState: {CurrentState}");
    }

    private bool IsInPreferredRange()
    {
        float dist = EntityProps.DistFromTarget;

        if(dist > min && dist < max)
        {
            return true;
        }

        TryKeepTargetPosInRange();
        CurrentState = PossibleStates[0].state;

        return false;
    }

    private void TryKeepTargetPosInRange()
    {
        float dist = EntityProps.DistFromTarget;

        SetRetreat(dist);
        CalculateTargetPos(dist);
    }

    private void SetRetreat(float dist)
    {
        if(dist < min && dist > -1)
        {
            isRetreating = true;
            EntityProps.MemorizedTargetPos = EntityProps.TargetTransform.position;
        }
        else if(dist > max)
        {
            isRetreating = false;
        }
    }

    private void CalculateTargetPos(float dist)
    {
        Vector2 current = EntityProps.Transform.position;
        Transform target = EntityProps.TargetTransform;

        if(target != null)
        {
            Vector2 targetPos = EntityProps.TargetTransform.position;
            Vector2 dirToTarget = (targetPos - current).normalized;
            float tollerantSpan = max - min;
            EntityProps.TargetPos = current + dirToTarget * (dist - max + tollerantSpan / 2);
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        if(CurrentState == PossibleStates[0].state)
        {
            TryKeepTargetPosInRange();
        }
        return base.GetCurrentState();
    }

    public override void Escape()
    {
        CurrentState = null;
        
        if(isRetreating)
        {
            EntityProps.TargetPos = EntityProps.MemorizedTargetPos;
            CurrentState = PossibleStates[1].state;
            isRetreating = false;
        }
        else
        {
            if(EntityProps.MemorizedTargetPos != null)
            {
                EntityProps.MemorizedTargetPos = null;
            }
            
            Context.Escape();
        }
    }
}
