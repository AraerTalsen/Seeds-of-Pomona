using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiationAffector : Affector
{
    public enum ParentSelect { none, host, target }
    [SerializeField] private GameObject node;
    [SerializeField] private ParentSelect parentSelection;
    public InstantiationAffector(Affector affcetor = null)
    {
        if(affcetor != null) (lifetimeLabel, lifetime, repeatLabel, runs, targetLabel, coordinator) = affcetor;
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
