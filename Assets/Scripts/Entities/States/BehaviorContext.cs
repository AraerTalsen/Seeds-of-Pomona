using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public abstract class BehaviorContext : BehaviorState, IBehaviorContext
{
    protected IBehaviorState currentState;
    public virtual IBehaviorState CurrentState { get => currentState; set => currentState = value; }

    public virtual List<(IBehaviorState state, int weight)> PossibleStates { get; } = new();

    public virtual void AddState(IBehaviorState state, int weight)
    {
        InitializeState(state);
        PossibleStates.Add((state, weight));
    }
    
    protected void InitializeStates()
    {
        for (int i = 0; i < PossibleStates.Count; i++)
        {
            IBehaviorState state = PossibleStates[i].state;
            
            if(state.EntityProps == null)
            {
                state.Context = this;
                state.EntityStateSupport = EntityStateSupport;
                state.EntityProps = EntityProps;
                if(state is BehaviorState behaviorState)
                {
                    behaviorState.TryLoadPowerupEffect();
                }
            }
        }
    }

    public void RemoveState(IBehaviorState state, int weight)
    {
        PossibleStates.Remove((state, weight));
    }

    protected void InitializeState(IBehaviorState state)
    {
        if(state.EntityProps == null)
        {
            state.Context = this;
            state.EntityStateSupport = EntityStateSupport;
            state.EntityProps = EntityProps;
        }
    }

    protected IBehaviorState ChooseRandomState()
    {
        int totalWeight = PossibleStates.Sum( pair => pair.state.IsValid ? pair.weight : 0 );
        int randNum = Random.Range(0, totalWeight);

        int currentWeight = 0;
        foreach((IBehaviorState state, int weight) in PossibleStates)
        {
            if(!state.IsValid || weight == 0) 
            {
                continue;
            }
           
            currentWeight += weight;
            if(randNum < currentWeight)
            {
                return state;
            }
        }

        return null;
    }

    public override abstract IEffectRuntime CreateEffectRuntime(EffectContext effectContext);
    public virtual void SelectNewState() => CurrentState = ChooseRandomState();
    public virtual IBehaviorState GetCurrentState() => CurrentState;
    public virtual void Escape()
    {
        CurrentState = null;
        Context?.Escape();
    }
}
