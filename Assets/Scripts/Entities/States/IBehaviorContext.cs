using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviorContext
{
    public IBehaviorState CurrentState { get; set; }
    public bool IsAggro { get; }
    public List<(IBehaviorState state, int weight)> PossibleStates { get; }
    public abstract void Escape();
    public abstract void AddToRegistry(IBehaviorContext context);
    public Dictionary<System.Type, IBehaviorContext> ContextRegistry { get; set; }
    public abstract void AddState(IBehaviorState state, int weight);
    public abstract void RemoveState(IBehaviorState state, int weight);
    public string Print();
}
