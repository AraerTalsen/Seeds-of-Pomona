using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Investigate")]
public class InvestigateState : BehaviorContext
{
    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    [BranchCondition] private bool IsSuspicious { get; set; }

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
            CurrentState = ChooseRandomState();
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        IsSuspicious = EntityProps.SuspiciousSpot == null;
        
        if(!IsSuspicious)
        {
            EntityProps.TargetPos = EntityProps.SuspiciousSpot;
            CurrentState = PossibleStates.Find(w => w.State is NavigateState).State;
        }

        return base.GetCurrentState();
    }
    
    public override void SelectNewState()
    {
        IsSuspicious = EntityProps.SuspiciousSpot == null;
        
        if(!IsSuspicious)
        {
            EntityProps.TargetPos = EntityProps.SuspiciousSpot;
            CurrentState = PossibleStates.Find(w => w.State is NavigateState).State;
        }
        else
        {
           base.SelectNewState();
        }
    }

    public override void Escape()
    {
        EntityProps.TargetPos = null;
        base.Escape();
    }
}
