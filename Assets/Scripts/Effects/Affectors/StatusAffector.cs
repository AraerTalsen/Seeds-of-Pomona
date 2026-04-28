using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAffector : Affector
{
    public enum StatusLabel { stun }

    [SerializeField] private StatusLabel statusLabel;
    
    public StatusAffector(Affector affector = null)
    {
        if(affector != null) (lifetimeLabel, lifetime, repeatLabel, runs, targetLabel, coordinator) = affector;
    }

    public override void Apply(EffectContext context)
    {
        throw new System.NotImplementedException();
    }

    public override IRuntimeEvent CreateRuntimeEvent(EffectContext context)
    {
        throw new System.NotImplementedException();
    }
}
