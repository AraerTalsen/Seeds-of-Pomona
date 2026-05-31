using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Enemy Behavior Base")]
public class EnemyBehaviorContext : BehaviorContext
{    
    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    public override float RecoveryTime { get; } = 8.0f;
    public override Dictionary<System.Type, IBehaviorContext> ContextRegistry { get; set; } = new();

    [BranchCondition] private bool IsResting { get; set; }
    [BranchCondition] private bool IsStunned { get; set; }
    [BranchCondition] private bool HasSpotted { get; set; }
    [BranchCondition] private bool RecallsTargetPos { get; set; }

    public void Initialize(EntityStateSupport entityStateSupport, EntityProperties entityProps)
    {
        EntityStateSupport = entityStateSupport;
        EntityProps = entityProps;

        CurrentState = ChooseRandomState();
        InitializeStates();
    }

    public override void SelectNewState()
    {
        HasSpotted = EntityStateSupport.CheckForTargetEntities();
        IsStunned = EntityProps.IsStunned;
        IsResting = EntityProps.IsResting;
        RecallsTargetPos = EntityProps.MemorizedTargetPos != null;
        
        if(!IsStunned)
        {
            if(!IsResting && !HasSpotted && !RecallsTargetPos)
            {
                CurrentState = ChooseRandomState();
            }
            else if(HasSpotted || RecallsTargetPos)
            {
                CurrentState = PossibleStates.Find(w => w.State is AggroState).State;
            }
            else if(IsResting)
            {
                CurrentState = null;
            }
        }
        else
        {
            CurrentState = null;
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        ReassessSystem();
        
        return base.GetCurrentState();
    }

    private void ReassessSystem()
    {
        HasSpotted = EntityStateSupport.CheckForTargetEntities();
        IsStunned = EntityProps.IsStunned;

        if(IsStunned || HasSpotted)
        {
            CurrentState = null;
        }

        if(HasSpotted)
        {
            EntityProps.SuspiciousSpot = null;
        }
    }
}