using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Sizing")]
public class SizingState : BehaviorContext
{
    private float min;
    private float max;
    public override bool IsAggro => true;

    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    private bool inRange;
    [BranchCondition] private bool InRange 
    { 
        get => inRange;
        set
        {
            inRange = value;
            InRangeTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float InRangeTimestamp { get; set; }
    [BranchCondition] private bool IsRetreating 
    { 
        get => EntityProps.IsRetreating;
        set
        {
            EntityProps.IsRetreating = value;
            IsRetreatingTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float IsRetreatingTimestamp { get; set; }

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
            IsValidTimestamp = Time.time;
        }
    }

    protected override bool DefaultNodePathValidity(BehaviorState node) => InRange && !IsRetreating && node.IsValid;
    public override void InitializeBranchValidityRec()
    {
        BehaviorState navigate = PossibleStates.Find(w => w.State is NavigateState).State;
        nodeValidityCheck.Add(navigate, s => !InRange && s.IsValid);
        branchValidity.Add(navigate, nodeValidityCheck[navigate](navigate));

        BehaviorState observe = PossibleStates.Find(w => w.State is ObserveState).State;
        nodeValidityCheck.Add(observe, s => DefaultNodePathValidity(s));
        branchValidity.Add(observe, nodeValidityCheck[observe](observe));

        List<BehaviorState> remainingStates = PossibleStates.Where( w => w.State != navigate && w.State != observe).Select( w => w.State).ToList();
        for(int i = 0; i < remainingStates.Count; i++)
        {
            BehaviorState state = remainingStates[i];
            nodeValidityCheck.Add(state, s => DefaultNodePathValidity(s));
            branchValidity.Add(state, nodeValidityCheck[state](state));
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
        CalculatePreferredPos(dist);
    }

    private void SetRetreat(float dist)
    {
        if(dist < min && dist > -1)
        {
            IsRetreating = true;
        }
        else if(dist > max)
        {
            IsRetreating = false;
        }
    }

    private void CalculatePreferredPos(float dist)
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
        if(CurrentState == PossibleStates.Find(w => w.State is NavigateState).State)
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
            Context.Escape();
        }
    }
}
