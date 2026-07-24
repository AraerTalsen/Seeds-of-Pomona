using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Aggro")]
public class AggroState : BehaviorContext
{
    private float max;
    private float tolerance;

    public override bool IsAggro => true;

    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    private EntityProperties entityProps;
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

    private bool hasTargetInSight;
    [BranchCondition] private bool HasTargetInSight
    {
        get => hasTargetInSight;
        set
        {
            hasTargetInSight = value;
            HasTargetTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float HasTargetTimestamp { get; set; }
    
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            ContextRegistry = Context.ContextRegistry;
            AddToRegistry(this);
            InitializeStates();
            CurrentState = PossibleStates.Find(w => w.State is PursuitState).State;
            tolerance = EntityProps.PreferredTolerance;
            max = EntityProps.PreferredRange.y;
            IsValidTimestamp = Time.time;
        }
    }

    protected override bool DefaultNodePathValidity(BehaviorState node) => InRange && node.IsValid;
    public override void InitializeBranchValidityRec()
    {
        BehaviorState pursuit = PossibleStates.Find(w => w.State is PursuitState).State;
        nodeValidityCheck.Add(pursuit, s => !InRange && s.IsValid);
        branchValidity.Add(pursuit, nodeValidityCheck[pursuit](pursuit));

        List<BehaviorState> remainingStates = PossibleStates.Where( w => w.State != pursuit).Select( w => w.State).ToList();
        for(int i = 0; i < remainingStates.Count; i++)
        {
            BehaviorState state = remainingStates[i];
            nodeValidityCheck.Add(state, s => DefaultNodePathValidity(s));
            branchValidity.Add(state, nodeValidityCheck[state](state));
        }
    }

    public override void SelectNewState()
    {
        HasTargetInSight = EntityProps.TargetTransform != null;
        InRange = EntityProps.DistFromTarget <= max + tolerance;
        if(!InRange)
        {
            CalculateTargetPos();
            CurrentState = PossibleStates.Find(w => w.State is PursuitState).State;
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
        Vector2 target = HasTargetInSight ? EntityProps.TargetTransform.position : (Vector2)EntityProps.TargetPos;
        Vector2 dirToTarget = (target - current).normalized;
        float magnitude = dist - max - tolerance + 0.1f;
        EntityProps.TargetPos = current + dirToTarget * magnitude;
    }

    public override IBehaviorState GetCurrentState()
    {
        InRange = EntityProps.DistFromTarget <= max + tolerance;
        HasTargetInSight = EntityProps.TargetTransform != null;
        
        if(!InRange)
        {
            CurrentState = null;
        }
        return base.GetCurrentState();
    }
}
