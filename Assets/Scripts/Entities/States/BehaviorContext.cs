using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public abstract class BehaviorContext : BehaviorState, IBehaviorContext
{
    protected IBehaviorState currentState;
    public virtual IBehaviorState CurrentState 
    { 
        get => currentState;
        set
        {
            currentState = value;
            if(currentState == null && EntityProps.IsVelocityVoid)
            {
                //EntityProps.NavMeshAgent.velocity = Vector2.zero;
                EntityProps.NavMeshAgent.isStopped = true;
            } 
        }
    }

    public virtual Dictionary<System.Type, IBehaviorContext> ContextRegistry { get; set; }

    public virtual List<(IBehaviorState state, int weight)> PossibleStates { get; } = new();
    public virtual bool IsAggro { get; }

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
        int index = PossibleStates.FindIndex( e => e.state.GetType().Equals(state.GetType()));
        PossibleStates.RemoveAt(index);
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
            //Debug.Log($"State {state} is valid {state.IsValid} and weighs more than 0: {weight > 0}");
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

        //EntityProps.NavMeshAgent.velocity = Vector2.zero;
        EntityProps.NavMeshAgent.isStopped = true;
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

    public virtual void AddToRegistry(IBehaviorContext context) => ContextRegistry.Add(context.GetType(), context);
}
