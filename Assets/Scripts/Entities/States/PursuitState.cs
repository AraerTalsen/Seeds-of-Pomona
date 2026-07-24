using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Behavior States/Contexts/Pursuit")]
public class PursuitState : BehaviorContext
{
    public override bool IsAggro => true;
    [SerializeField] private List<IBehaviorContext.WeightedState> possibleStates = new();
    public override List<IBehaviorContext.WeightedState> PossibleStates { get => possibleStates; set => possibleStates = value; }

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
            IsValidTimestamp = Time.time;
        }
    }

    protected override bool DefaultNodePathValidity(BehaviorState node) =>  node.IsValid;
    public override void InitializeBranchValidityRec()
    {
        List<BehaviorState> remainingStates = PossibleStates.Select( w => w.State).ToList();
        for(int i = 0; i < remainingStates.Count; i++)
        {
            BehaviorState state = remainingStates[i];
            nodeValidityCheck.Add(state, s => DefaultNodePathValidity(s));
            branchValidity.Add(state, nodeValidityCheck[state](state));
        }
    }
}
