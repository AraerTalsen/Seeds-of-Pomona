using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorContext : BehaviorState, IBehaviorContext
{
    public virtual IBehaviorState CurrentState { get; set; }
    public virtual List<(IBehaviorState state, int maxThreshold)> PossibleStates { get; }

    protected void InitializeStates()
    {
        for (int i = 0; i < PossibleStates.Count; i++)
        {
            IBehaviorState state = PossibleStates[i].state;
            state.Context = this;
            state.EntityStateSupport = EntityStateSupport;
            state.EntityProps = EntityProps;
        }
    }
}
