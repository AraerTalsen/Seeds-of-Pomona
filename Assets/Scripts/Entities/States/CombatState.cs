using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Combat")]
public class CombatState : BehaviorContext
{
    private BehaviorState preparedMove = null;
    public override bool IsAggro => true;

    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }
    private Dictionary<IBehaviorState, bool> moveSetExhaustian = new();
    [BranchCondition] public override bool IsValid => !AllMovesExhausted();
    [BranchCondition] private bool InRange { get; set; }

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
        }
    }

    public override void AddState(BehaviorState state, int weight)
    {
        base.AddState(state, weight);
        if(state.IsAttack) moveSetExhaustian.Add(state, false);
    }

    private bool AllMovesExhausted()
    {
        foreach(KeyValuePair<IBehaviorState, bool> pair in moveSetExhaustian)
        {
            if(!pair.Value) return false;
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
        InRange = dist - range > 0.1f;
        
        if(InRange)
        {
            Vector2 current = EntityProps.Transform.position;
            Vector2 dirToTarget = ((Vector2)EntityProps.TargetTransform.position - current).normalized;
            EntityProps.TargetPos = current + dirToTarget * (dist - range);
        }
    }

    public void UpdateMoveSetExhaustian(IBehaviorState state, bool isExhausted) => moveSetExhaustian[state] = isExhausted;

    public override void SelectNewState() => PrepareMove();

    public override void Escape()
    {
        CurrentState = preparedMove;
        preparedMove = null;
        if(CurrentState == null)
        {
            Context.Escape();
        }
        else
        {
            UpdateMoveSetExhaustian(CurrentState, true);
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        CalculatePosition();
        return base.GetCurrentState();
    }

    private void InitializeMoveSet()
    {
        for(int i = 0; i < PossibleStates.Count; i++)
        {
            BehaviorState state = PossibleStates[i].State;
            if(state.IsAttack) moveSetExhaustian.Add(state, false);
        }
    }
}
