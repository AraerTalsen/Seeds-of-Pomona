using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public abstract class BehaviorContext : BehaviorState, IBehaviorContext
{
    protected BehaviorState currentState;
    public virtual BehaviorState CurrentState 
    { 
        get => currentState;
        set 
        {
            currentState = value; 
        }
    }

    public virtual Dictionary<Type, IBehaviorContext> ContextRegistry { get; set; }

    public virtual List<IBehaviorContext.WeightedState> PossibleStates { get; set; } = new();
    public virtual bool IsAggro { get; }
    public int RandomRoll { get; set; } = 0;

    //Debugging data for Node Tree Display
    public Dictionary<BehaviorState, bool> branchValidity = new();
    public Dictionary<BehaviorState, Func<BehaviorState, bool>> nodeValidityCheck = new();

    public virtual void AddState(BehaviorState state, int weight)
    {
        InitializeState(state);
        PossibleStates.Add(new (state, weight));

        nodeValidityCheck.Add(state, DefaultNodePathValidity);
        branchValidity.Add(state, nodeValidityCheck[state](state));
    }
    
    protected void InitializeStates()
    {
        for (int i = 0; i < PossibleStates.Count; i++)
        {
            IBehaviorState state = PossibleStates[i].State;
            
            if(state.EntityProps == null)
            {
                state.Context = this;
                state.EntityStateSupport = EntityStateSupport;
                state.EntityProps = EntityProps;
            }
        }
    }

    public void RemoveState(BehaviorState state, int weight)
    {
        int index = PossibleStates.FindIndex( e => e.State.GetType().Equals(state.GetType()));
        PossibleStates.RemoveAt(index);
    }

    protected void InitializeState(BehaviorState state)
    {
        if(state.EntityProps == null)
        {
            state.Context = this;
            state.EntityStateSupport = EntityStateSupport;
            state.EntityProps = EntityProps;
        }
    }

    protected BehaviorState ChooseRandomState()
    {
        int totalWeight = PossibleStates.Sum( pair => pair.State.IsValid ? pair.Weight : 0 );
        RandomRoll = UnityEngine.Random.Range(0, totalWeight);

        int currentWeight = 0;
        
        foreach((BehaviorState state, int weight) in PossibleStates)
        {
            currentWeight += weight;
            if(!state.IsValid || weight == 0) continue;
           
            if(RandomRoll < currentWeight)
            {
                return state;
            }
        }

        return null;
    }

    public virtual void SelectNewState() => CurrentState = ChooseRandomState();
    public virtual IBehaviorState GetCurrentState() 
    { 
        NodeTimestamp = Time.time;
        return CurrentState;
    }
    public virtual void Escape()
    {
        CurrentState = null;
        Context?.Escape();
    }

    public virtual void AddToRegistry(IBehaviorContext context) => ContextRegistry.Add(context.GetType(), context);

    public string Print()
    {
        int layer = 1;
        BehaviorContext layerContext = Context;

        while(layerContext != null)
        {
            layerContext = layerContext.Context;
            layer++;
        }

        string depDash = new('-', layer);
        string currentDirectory = "";

        foreach((IBehaviorState state, _) in PossibleStates)
        {
            currentDirectory += depDash + state + "\n";
            if(state is IBehaviorContext context)
            {
                currentDirectory += context.Print();
            }
        }
        
        return currentDirectory;
    }

    //Debugging functions for Node Tree Display
    protected abstract bool DefaultNodePathValidity(BehaviorState node);
    public abstract void InitializeBranchValidityRec(); 
    public void UpdateBranchValidityRec()
    {
        for(int i = 0; i < branchValidity.Count; i++)
        {
            (BehaviorState state, _) = branchValidity.ElementAt(i);
            branchValidity[state] = nodeValidityCheck[state](state);
        }
    }
}
