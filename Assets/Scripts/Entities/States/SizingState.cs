using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Sizing")]
public class SizingState : BehaviorContext
{
    private float min;
    private float max;
    public override bool IsAggro => true;

    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    [BranchCondition] private bool InRange { get; set; }
    [BranchCondition] private bool IsRetreating { get; set; }

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
            CurrentState = PossibleStates.Find(w => w.State is NavigateState).State;
            min = EntityProps.PreferredRange.x;
            max = EntityProps.PreferredRange.y;
        }
    }

    public override void SelectNewState()
    {
        if(IsInPreferredRange())
        {
            CurrentState = ChooseRandomState();
        }
    }

    private bool IsInPreferredRange()
    {
        float dist = EntityProps.DistFromTarget;

        InRange = dist > min && dist < max;
        if(InRange)
        {
            return true;
        }

        TryKeepTargetPosInRange();
        CurrentState = PossibleStates.Find(w => w.State is NavigateState).State;

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
            IsRetreating = true;
            EntityProps.MemorizedTargetPos = EntityProps.TargetTransform.position;
        }
        //Do we need this condition? I think the entity should arrive at its target before it can realize it should no longer retreat
        else if(dist > max)
        {
            IsRetreating = false;
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
        if(CurrentState == PossibleStates[0].State)
        {
            TryKeepTargetPosInRange();
        }
        return base.GetCurrentState();
    }

    public override void Escape()
    {
        CurrentState = null;
        
        if(IsRetreating)
        {
            EntityProps.TargetPos = EntityProps.MemorizedTargetPos;
            CurrentState = PossibleStates.Find(w => w.State is ObserveState).State;
            IsRetreating = false;
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
