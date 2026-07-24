using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Investigate")]
public class InvestigateState : BehaviorContext
{
    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

    private bool isSuspicious;
    [BranchCondition] private bool IsSuspicious 
    { 
        get => isSuspicious;
        set
        {
            isSuspicious = value;
            IsSuspiciousTimestamp = Time.time;
        }
    }
    [ConditionTimestamp] private float IsSuspiciousTimestamp { get; set; }

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
            IsValidTimestamp = Time.time;
        }
    }

    protected override bool DefaultNodePathValidity(BehaviorState node) => !IsSuspicious && node.IsValid;
    public override void InitializeBranchValidityRec()
    {
        BehaviorState navigate = PossibleStates.Find(w => w.State is NavigateState).State;
        nodeValidityCheck.Add(navigate, s => s.IsValid);
        branchValidity.Add(navigate, nodeValidityCheck[navigate](navigate));

        List<BehaviorState> remainingStates = PossibleStates.Where( w => w.State != navigate).Select( w => w.State).ToList();
        for(int i = 0; i < remainingStates.Count; i++)
        {
            BehaviorState state = remainingStates[i];
            nodeValidityCheck.Add(state, s => DefaultNodePathValidity(s));
            branchValidity.Add(state, nodeValidityCheck[state](state));
        }
    }

    public override IBehaviorState GetCurrentState()
    {
        IsSuspicious = EntityProps.SuspiciousSpot != null;
        
        if(IsSuspicious)
        {
            EntityProps.TargetPos = EntityProps.SuspiciousSpot;
            CurrentState = PossibleStates.Find(w => w.State is NavigateState).State;
        }

        return base.GetCurrentState();
    }
    
    public override void SelectNewState()
    {
        IsSuspicious = EntityProps.SuspiciousSpot != null;
        
        if(IsSuspicious)
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
