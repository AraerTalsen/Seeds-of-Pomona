using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatAffector : Affector
{
    [SerializeField] private Stats statType;
    [SerializeField] private bool isPositive;
    [SerializeField] private int magnitude;
    
    public StatAffector(Affector affector = null)
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
