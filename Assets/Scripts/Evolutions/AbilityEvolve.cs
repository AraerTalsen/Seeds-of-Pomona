using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Evolutions")]
public class AbilityEvolve : Evolutions, IBehaviorState
{
    [HideInInspector]
    public IBehaviorContext Context { get; set; }
    [HideInInspector]
    public EntityProperties EntityProps { get; set; }
    [HideInInspector]
    public EntityStateSupport EntityStateSupport { get; set; }

    public float RecoveryTime { get; }

    public virtual void PerformAction()
    {
        throw new System.NotImplementedException();
    }
}
