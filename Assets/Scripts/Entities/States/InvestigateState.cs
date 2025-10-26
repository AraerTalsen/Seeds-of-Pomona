using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateState : BehaviorContext
{
    public override List<(IBehaviorState state, int maxThreshold)> PossibleStates { get; } = new() { (new ObserveState(), 67), (new NavigateState(), 100) };

    private EntityProperties entityProps;
    public override EntityProperties EntityProps
    {
        get => entityProps;
        set
        {
            entityProps = value;
            InitializeStates();
        }
    }

    public override void PerformAction()
    {
        if (CurrentState == null)
        {
            ChooseState();
        }
        CurrentState.PerformAction();
    }

    private void ChooseState()
    {
        int rand = Random.Range(0, 101);

        for (int i = 0; i < PossibleStates.Count; i++)
        {
            if (PossibleStates[i].maxThreshold >= rand)
            {
                CurrentState = PossibleStates[i].state;
                break;
            }
        }
    }
}
