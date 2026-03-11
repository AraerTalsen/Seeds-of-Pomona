using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : BehaviorContext
{
    private IBehaviorState preparedMove = null;

    public override List<(IBehaviorState state, int weight)> PossibleStates { get; } = new() 
    { 
        (new NavigateState(), 0),
        (new BasicMeleeState(), 1)
    };
    private Dictionary<IBehaviorState, bool> moveSetExhaustian = new();
    public override bool IsValid => !AllMovesExhausted();

    private EntityProperties entityProps;
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            InitializeStates();
            CurrentState = null;
            moveSetExhaustian.Add(PossibleStates[1].state, false);
        }
    }

    public override void AddState(IBehaviorState state, int weight)
    {
        base.AddState(state, weight);
        moveSetExhaustian.Add(state, false);
    }

    private bool AllMovesExhausted()
    {
        foreach(KeyValuePair<IBehaviorState, bool> pair in moveSetExhaustian)
        {
            if(!pair.Value) return false;
        }
        return true;
    }

    public override IEffectRuntime CreateEffectRuntime(EffectContext effectContext) => null;

    private void PrepareMove()
    {
        preparedMove = ChooseRandomState();
        MoveToPosition();
    }

    private void MoveToPosition()
    {
        CalculatePosition();
        CurrentState = PossibleStates[0].state;
    }

    private void CalculatePosition()
    {
        float range = EntityProps.MeleeRange - 0.1f;
        float dist = EntityProps.DistFromTarget;
        
        if(dist - EntityProps.MeleeRange > 0.1f)
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
    }

    public override IBehaviorState GetCurrentState()
    {
        CalculatePosition();
        return base.GetCurrentState();
    }
    //Create SetCurrentState function to ensure that every state set to current is valid
}
