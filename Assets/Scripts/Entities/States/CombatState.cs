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
    private Dictionary<IBehaviorState, bool> moveSetExhaustian = new() { { new BasicMeleeState(), false } };
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
        float range = EntityProps.MeleeRange;
        if(EntityProps.DistFromTarget - EntityProps.MeleeRange > 0.1f)
        {
            Vector2 dirToTarget = (EntityProps.Transform.position - EntityProps.TargetTransform.position).normalized;
            EntityProps.TargetPos = dirToTarget * range;
            CurrentState = PossibleStates[1].state;
        }
        else
        {
            CurrentState = preparedMove;
            preparedMove = null;
            if(CurrentState == null)
            {
                EntityProps.Rigidbody.velocity = Vector2.zero;
            }
        }
        
    }

    public void UpdateMoveSet(IBehaviorState state, bool isExhausted) => moveSetExhaustian[state] = isExhausted;

    public override void SelectNewState() => PrepareMove();

    public override void Escape()
    {
        CurrentState = null;
        if(preparedMove == null)
        {
            Context.Escape();
        }
    }
    //Create SetCurrentState function to ensure that every state set to current is valid
}
