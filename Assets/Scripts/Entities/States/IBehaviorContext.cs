using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviorContext
{
    [System.Serializable]
    public struct WeightedState
    {
        [SerializeField] private BehaviorState _state;
        [SerializeField] private int _weight;
        public BehaviorState State => _state;
        public int Weight { get => _weight; set => _weight = value; }

        public WeightedState(BehaviorState state, int weight)
        {
            _state = state;
            _weight = weight;
        }

        public WeightedState(WeightedState weightedState)
        {
            _state = weightedState.State;
            _weight = weightedState.Weight;
        }

        public void Deconstruct(out BehaviorState state, out int weight)
        {
            state = _state;
            weight = _weight;
        }
    }
    
    public BehaviorState CurrentState { get; set; }
    public bool IsAggro { get; }
    public List<WeightedState> PossibleStates { get; }
    public abstract void Escape();
    public abstract void AddToRegistry(IBehaviorContext context);
    public Dictionary<System.Type, IBehaviorContext> ContextRegistry { get; set; }
    public abstract void AddState(BehaviorState state, int weight);
    public abstract void RemoveState(BehaviorState state, int weight);
    public string Print();
}
