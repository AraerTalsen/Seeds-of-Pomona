using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Combat")]
public class CombatState : BehaviorContext
{
    private BehaviorState preparedMove = null;
    public override bool IsAggro => true;

    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }
    private Dictionary<IBehaviorState, bool> moveSetExhaustian = new();
    [BranchCondition] public override bool IsValid
    {
        get
        {
            IsValidTimestamp = Time.time;
            return !AllMovesExhausted();
        }
    }
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
            CurrentState = null;
            InitializeMoveSet();
            IsValidTimestamp = Time.time;
        }
    }

    protected override bool DefaultNodePathValidity(BehaviorState node) => InRange && node.IsValid;
    public override void InitializeBranchValidityRec()
    {
        BehaviorState navigate = PossibleStates.Find(w => w.State is NavigateState).State;
        nodeValidityCheck.Add(navigate, s => !InRange && s.IsValid);
        branchValidity.Add(navigate, nodeValidityCheck[navigate](navigate));

        List<BehaviorState> remainingStates = PossibleStates.Where( w => w.State != navigate).Select( w => w.State).ToList();
        for(int i = 0; i < remainingStates.Count; i++)
        {
            BehaviorState state = remainingStates[i];
            nodeValidityCheck.Add(state, s => DefaultNodePathValidity(s));
            branchValidity.Add(state, nodeValidityCheck[state](state));
        }
    }

    public override void AddState(BehaviorState state, int weight)
    {
        base.AddState(state, weight);
        if(state.IsSpecial) moveSetExhaustian.Add(state, !state.IsValid);
    }

    private bool AllMovesExhausted()
    {
        foreach((IBehaviorState state, _) in moveSetExhaustian)
        {
            if(state.IsValid) return false;
        }
        return true;
    }

    private void PrepareMove()
    {
        preparedMove = ChooseRandomState();
        MoveToPosition();
    }

    private void MoveToPosition()
    {
        CalculatePosition();
        CurrentState = PossibleStates.Find(w => w.State is NavigateState).State;
    }

    private void CalculatePosition()
    {
        float range = EntityProps.MeleeRange - 0.1f;
        float dist = EntityProps.DistFromTarget;
        InRange = dist - range < 0.1f;
        
        if(!InRange)
        {
            Vector2 current = EntityProps.Transform.position;
            Vector2 dirToTarget = ((Vector2)EntityProps.TargetTransform.position - current).normalized;
            EntityProps.TargetPos = current + dirToTarget * (dist - range);
        }
    }

    public override void SelectNewState() => PrepareMove();

    public override void Escape()
    {
        CurrentState = EntityProps.IsTargetLost ? null : preparedMove;
        preparedMove = null;
        if(CurrentState == null)
        {
            Context.Escape();
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        CalculatePosition();
        if(!InRange) CurrentState = null;
        return base.GetCurrentState();
    }

    private void InitializeMoveSet()
    {
        for(int i = 0; i < PossibleStates.Count; i++)
        {
            BehaviorState state = PossibleStates[i].State;
            if(state.IsSpecial) moveSetExhaustian.Add(state, !state.IsValid);
        }
    }
}
