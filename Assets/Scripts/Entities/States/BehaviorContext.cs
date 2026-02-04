using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BehaviorContext : BehaviorState, IBehaviorContext
{
    public virtual IBehaviorState CurrentState { get; set; }
    public virtual List<(IBehaviorState state, int weight)> PossibleStates { get; } = new();

    public void AddState(IBehaviorState state, int weight)
    {
        InitializeState(state);
        PossibleStates.Add((state, weight));
    }
    
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

    protected void InitializeState(IBehaviorState state)
    {
        state.Context = this;
        state.EntityStateSupport = EntityStateSupport;
        state.EntityProps = EntityProps;
    }

    protected IBehaviorState ChooseRandomState()
    {
        int totalWeight = PossibleStates.Sum(state => state.weight);
        int randNum = Random.Range(0, totalWeight);

        int currentWeight = 0;
        foreach((IBehaviorState state, int weight) in PossibleStates)
        {
           currentWeight += weight;

            if(randNum < currentWeight)
            {
                return state;
            }
        }

        return default;
    }
}
