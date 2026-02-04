using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviorContext
{
    public IBehaviorState CurrentState { get; set; }
    public List<(IBehaviorState state, int weight)> PossibleStates { get; }
}
